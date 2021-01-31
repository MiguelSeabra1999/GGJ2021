using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;



public class CharacterController : MonoBehaviour
{
    // Start is called before the first frame update


    private Camera m_camera;
    private GameObject postProcessor;
    public bool camera_follow = true;

    private KeyBindings keyBindings;
    private Rigidbody2D rb;
    private Collider2D coll;
    public float speed;
    public Vector3 jump;
    public float jumpForce = 2.0f;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public float aimSpeed = 15;
    public float dashTime = 0.5f;
    public float dashSpeedup = 1.5f;
    public float dashInvincibilityTime = 0.2f;

    public float midairDirecitonChangeSpeed = 0.3f;

    public float shootingCooldown = 0.3f;
    public float dashingCooldown = 0.3f;

    public float invincibilityTime = 0.3f;
    public float blinkSpeed = 0.1f;
    public float stompJumpForceMultiplier = 1.5f;
    public float stompSpread = 0.4f;

    
    public Vector3 ModuleOffSet =new Vector3 (1,1,0);
    public float moduleJumpForce = 1;

    public Vector3 dashingDir = Vector3.zero;
    public int jumpDir = 1;


    
    public bool isGrounded = true;
    public bool dashing = false;
    public bool facingForward = true;

    public bool canShoot = true;
    public bool canDash = true;

    public bool isInvincible;

    public GameObject neckJoint;
    public GameObject armJoint;

    public GameObject GunPoint;
    public GameObject ParticleSpawn;
    public GameObject LandParticleSpawn;

    public GameObject BulletPrefab;
    public GameObject ModulePrefab;
    public GameObject DustParticlePrefab;
    public GameObject LandDustParticlePrefab;

    public GameObject SmokeBombPrefab;
    public ParticleSystem ReturnShotParticles;
    public ParticleSystem ReturnDashParticles;
    public float BulletSpawnOffset = 50;
    private Animator animator;

    [HideInInspector]public SpriteRenderer[] sprites;


    //Audio
    [FMODUnity.EventRef]
    public string jumpSFX = string.Empty;
    [FMODUnity.EventRef]
    public string landSFX = string.Empty;
    [FMODUnity.EventRef]
    public string walkSFX = string.Empty;

    void Start()
    {
        keyBindings = new KeyBindings(this.gameObject);
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        m_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        postProcessor = GameObject.Find("PostProcessor");
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        Debug.Log(sprites.Length);
        Instantiate(SmokeBombPrefab, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if(camera_follow)
            m_camera.transform.position = new Vector3 (transform.position.x + 0, transform.position.y + 0, m_camera.transform.position.z); // Camera follows the player with specified offset position
        
        CheckStomp();
        if(CheckGround())
            isGrounded = true;
        else{
            if( animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") || animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") )
                animator.Play("Jump");
            isGrounded = false;
        }
        keyBindings.UpdateKeys();
        (float, bool) LookInfo = keyBindings.GetLookAngle();
        float lookAngle = LookInfo.Item1;
        bool lookingBack = LookInfo.Item2;
        Vector3 dir = keyBindings.GetDirection() *speed * Time.deltaTime;

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if(dashing)
        {
            UpdateDash();
            return;
        }

        
        if(dir.magnitude > 0.01f)
            UpdateMovement(dir);
        else
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Stun") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
                animator.Play("Idle");


        if(keyBindings.Keys["Jump"] && isGrounded)
            Jump();
        if(keyBindings.Keys["Shoot"] && canShoot)
            Shoot();
        if(keyBindings.Keys["Dodge"] && canDash)
            Dash();

        UpdateLookDirection(lookAngle, lookingBack);
    }   

    private void UpdateMovement(Vector3 dir)
    {
        //Debug.Log(dir.x);
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Stun") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
                animator.Play("Walk");
            if(!isGrounded && jumpDir == 1 && dir.x <0   ||  !isGrounded && jumpDir == -1 && dir.x >0 )
                 dir = dir*midairDirecitonChangeSpeed;


            if(facingForward)
            {
               // rb.velocity = new Vector2(dir.x,0);
                this.transform.Translate(dir.x  ,0,0);
            }
            else
            {

                this.transform.Translate(-1*dir.x  ,0,0);
            }
            //FMODUnity.RuntimeManager.PlayOneShot(walkSFX);
            //sound-walk;
            if(isGrounded && Random.Range(0f,1f) < 0.02f )
                Instantiate(DustParticlePrefab, ParticleSpawn.transform.position, Quaternion.identity);

    }
    private void Shoot()
    {
        Instantiate(BulletPrefab, GunPoint.transform.position, GunPoint.transform.rotation);
        canShoot = false;
        Invoke("RestoreShoot", shootingCooldown);
    }   
    private void Jump()
    {
//        Debug.Log("FF");
        //sound-jump
        animator.Play("PreJump");
        if(facingForward)
            jumpDir = 1;
        else    
            jumpDir = -1;
        
         rb.AddForce(jump * jumpForce, ForceMode2D.Impulse);
         isGrounded = false;

        //Audio
        FMODUnity.RuntimeManager.PlayOneShot(jumpSFX);
    }

    private void Jump(float mod)
    {
        animator.Play("PreJump");
        //sound-OnEnemyHead
        if(facingForward)
            jumpDir = 1;
        else    
            jumpDir = -1;
        
         rb.AddForce(jump * jumpForce * mod, ForceMode2D.Impulse);
         isGrounded = false;
        //Audio
        FMODUnity.RuntimeManager.PlayOneShot(jumpSFX);
    }


    private void Dash()
    {
      //  Debug.Log("DashStart");
        canDash = false;

        this.StartCoroutine(Dashing());
    }

    IEnumerator Dashing()
    {
  //      Debug.Log("coroutine");
        animator.SetBool("Dodging", true);
        this.dashing = true;
        dashingDir = keyBindings.GetLookDir().normalized;
        if(isGrounded)
        {
                if(dashingDir.y <= 0)
                {
                    if(facingForward)
                    dashingDir = new Vector3(1,0,0);
                    else
                    dashingDir = new Vector3(-1,0,0);
                }
  
        }
    
        yield return new WaitForSeconds(dashTime);

        
        this.dashing = false;
        rb.velocity = Vector2.zero;
         animator.SetBool("Dodging", false);
        Invoke("RestoreDash", dashingCooldown);
    }
    private void RestoreDash()
    {
        ReturnDashParticles.Play();
        this.StartCoroutine(InvincibilityLifeCycle(dashInvincibilityTime));
        canDash = true;
    }

    private void RestoreShoot()
    {
        ReturnShotParticles.Play();
        canShoot = true;
    }

    private void UpdateDash()
    {
      //  Debug.Log("update");
      if(dashing)
        //this.transform.Translate(dashingDir.x * speed * Time.deltaTime * dashSpeedup,dashingDir.y * speed * Time.deltaTime * dashSpeedup,0);
        rb.velocity = new Vector2(dashingDir.x * speed * dashSpeedup ,dashingDir.y * speed * dashSpeedup);
    }

    private void Damage(GameObject other)
    {
        if(dashing && other != null)
        {
            SendDamage(other, 1f);
            return;
        }

        if(isInvincible) return;
            
        Debug.Log("ouch");
        PostProcessorInterface.DamageEffect(0.4f);
        ModuleType type = keyBindings.RemoveRandomKey() ;
        this.StartCoroutine(InvincibilityLifeCycle(invincibilityTime));
        if(type == ModuleType.EMPTY || keyBindings.NoActiveKeys())
        {
            Destroy(gameObject);
        }else
        {
            if(other.transform.position.x > transform.position.x)
                SpawnModule(type ,false);
            else 
                SpawnModule(type ,true);
        }
    }

    private void Damage_null(GameObject other)
    { 
        if(isInvincible || dashing) return;
            
        Debug.Log("ouch");
        PostProcessorInterface.DamageEffect(0.4f);
        ModuleType type = keyBindings.RemoveRandomKey() ;
        this.StartCoroutine(InvincibilityLifeCycle(invincibilityTime));
        if(type == ModuleType.EMPTY || keyBindings.NoActiveKeys())
        {
            Destroy(gameObject);
        }else
        {
            if(other.transform.position.x > transform.position.x)
                SpawnModule(type ,false);
            else 
                SpawnModule(type ,true);
        }
    }

    private void SpawnModule(ModuleType type, bool invertDir)
    {
        Vector3 offset = ModuleOffSet;
        if(invertDir)
            offset = new Vector3(offset.x*-1,offset.y,0);
        GameObject newModule = Instantiate(ModulePrefab, transform.position + offset, Quaternion.identity );
        newModule.SendMessage("SetModuleType", type);
        Rigidbody2D moduleRb = newModule.GetComponent<Rigidbody2D>();
        moduleRb.AddForce(offset * moduleJumpForce, ForceMode2D.Impulse);
    }

    private void SendDamage(GameObject obj, float damage)
    {
            Enemy target = obj.GetComponent<Enemy>();
            target.HP-=damage;
            if(target.HP<=0){
                obj.SendMessage("Stomped");
            }
    }


    private void UpdateLookDirection(float lookAngle, bool lookingBack)
    {
        //Debug.Log(lookingBack);
        if(lookingBack)
        {
            if(facingForward)
                Flip();
           //lookAngle +=180;
            
        }else if(!facingForward)
        {
            Flip();
     
        }

       // armJoint.transform.LookAt(transform.position + lookDir);
        Quaternion targetRotationArm = Quaternion.Euler(0,0,lookAngle );
     /*   if(lookingBack)
            targetRotationArm = Quaternion.Euler(0,0,-1*lookAngle - 90);*/

        Quaternion targetRotationHead = Quaternion.Euler(0,0,(lookAngle));
       /* if(lookingBack)
            targetRotationHead = Quaternion.Euler(0,0,-1*lookAngle );*/

        armJoint.transform.localRotation = Quaternion.Lerp(armJoint.transform.localRotation, targetRotationArm, Time.deltaTime * aimSpeed );
        neckJoint.transform.localRotation = Quaternion.Lerp(neckJoint.transform.localRotation, targetRotationHead, Time.deltaTime * aimSpeed );
    }


    private void Flip()
    {
   //  Debug.Log("Flip");
        if(transform.rotation.eulerAngles.y == 0 )
        {
            transform.rotation =  Quaternion.Euler(0,180,0); 
            facingForward = false;
        }
        else 
        {
            transform.rotation =  Quaternion.Euler(0,0,0); 
            facingForward = true;
        }
    }

    private void TouchGround()
    {
        Debug.Log("Ground");
        isGrounded = true;
        //Audio
        //FMODUnity.RuntimeManager.PlayOneShot(landSFX);
    }

    private void TouchWall()
    {
        dashing = false;
        
    }

    private void CollectModule(GameObject ModuleObj)
    {
        Module module = ModuleObj.GetComponent<Module>();
        //do stuff
        switch(module.type)
        {
            case ModuleType.MOVEMENT:
                keyBindings.SetActiveKeys("Movement", true);
            break;
            case ModuleType.DASH:
                keyBindings.SetActiveKeys("Dodge", true);
            break;
            case ModuleType.JUMP:
                keyBindings.SetActiveKeys("Jump", true);
            break;
            case ModuleType.SHOOT:
                keyBindings.SetActiveKeys("Shoot", true);
            break;
        }
        
        Destroy(ModuleObj);
    }


    private void OnCollisionEnter2D(Collision2D col) 
    {
       // Debug.Log(col.otherCollider.gameObject.name);
          /*  List<ContactPoint2D> contacts = new List<ContactPoint2D>();
            col.GetContacts(contacts);
           // if(col.otherCollider.gameObject.name == "Feet")
            if(col.gameObject.layer ==10)
                foreach(ContactPoint2D point in contacts)
                    if(CheckIfTouchingWithFeet(transform.position, point.point))*/
              /*  if(col.gameObject.layer ==10)
                            TouchGround();*/
            if(col.gameObject.layer ==8 || col.gameObject.layer ==11)
                TouchWall();
            if(col.gameObject.layer ==11)
                CollectModule(col.gameObject);
    }



    private void TouchWithFeet(int layerCode)
    {
        Debug.Log("Touvhing");
        if(layerCode ==10)
            TouchGround();
    }

   /* private void OnTriggerEnter(Collider other) {
        Debug.Log("Ouch");
    }*/

    private bool CheckIfTouchingWithFeet(Vector2 from, Vector2 to)
    {
        Vector2 diff = to - from;
        diff = diff.normalized;
       // float sin = Mathf.Asin(diff.y)*Mathf.Rad2Deg;
       Debug.Log(diff.x);
        if(Mathf.Abs(diff.x) < 0.5f)
        {
            
            return true;
        }
        return false;
    }


    IEnumerator InvincibilityLifeCycle(float time)
    {
        isInvincible = true;
        gameObject.layer = 14;
        float startTime = Time.time;
        float timePercent;
        if(time == invincibilityTime)
         animator.SetBool("Stuned",true);
       // yield return new WaitForSeconds(invincibilityTime);
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        while(Time.time < startTime + time)
        {
            timePercent =(Time.time - startTime)  / time;
            SetSpriteActive(false, sprites);
            yield return new WaitForSeconds(blinkSpeed);
            SetSpriteActive(true,sprites);
            yield return new WaitForSeconds(blinkSpeed);
            if(time == invincibilityTime && animator.GetBool("Stuned"))
                animator.SetBool("Stuned",false);

        }

       /* if(time == invincibilityTime)
         animator.SetBool("Stuned",false);*/
        isInvincible = false;
        gameObject.layer =7;
        //coll.enabled = true;
    }
    
    private void SetSpriteActive(bool state, SpriteRenderer[] sprites)
    {
     //   Debug.Log(sprites.Length);
  
        foreach(SpriteRenderer sprite in sprites)
        {
            if(sprite != null)
            sprite.enabled = state;
        }
    }
    private bool CheckGround()
    {
       //return true;
        float floorDist = 1.4f;
        LayerMask mask = LayerMask.GetMask("Floor");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, floorDist, mask);
        Debug.DrawRay(transform.position, -Vector3.up * floorDist,Color.blue);
        if(hit.collider != null && hit.collider.gameObject.layer == 10)
        {
           // Debug.Log(hit.collider.gameObject.name);
            if(!isGrounded)
            {
                //sound-landSound
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                    animator.Play("Idle");
                FMODUnity.RuntimeManager.PlayOneShot(landSFX);
                Instantiate(LandDustParticlePrefab, LandParticleSpawn.transform.position, Quaternion.identity);
            }
            return true;
        }

        return false;
    }
    private bool CheckStomp()
    {
       //return true;
        if(isInvincible)return false;
        float floorDist = 1.3f;
        LayerMask mask = LayerMask.GetMask("Enemy");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, floorDist, mask);
        Debug.DrawRay(transform.position, -Vector3.up * floorDist,Color.yellow);
        if(hit.collider != null && hit.collider.gameObject.layer == 9)
        {
            SendDamage(hit.collider.gameObject,1);
            rb.velocity = Vector2.zero;
            Jump(stompJumpForceMultiplier);
            return true;
        }
        hit = Physics2D.Raycast(transform.position + new Vector3(stompSpread,0,0), -Vector2.up, floorDist, mask);
        Debug.DrawRay(transform.position+ new Vector3(stompSpread,0,0), -Vector3.up * floorDist,Color.yellow);
        if(hit.collider != null && hit.collider.gameObject.layer == 9)
        {
            SendDamage(hit.collider.gameObject,1);
            rb.velocity = Vector2.zero;
            Jump(stompJumpForceMultiplier);
            return true;
        }
        hit = Physics2D.Raycast(transform.position+ new Vector3(-1*stompSpread,0,0), -Vector2.up, floorDist, mask);
        Debug.DrawRay(transform.position+ new Vector3(-1*stompSpread,0,0), -Vector3.up * floorDist,Color.yellow);
        if(hit.collider != null && hit.collider.gameObject.layer == 9)
        {
            SendDamage(hit.collider.gameObject,1);
            rb.velocity = Vector2.zero;
            Jump(stompJumpForceMultiplier);
            return true;
        }

        return false;
    }

    public bool CheckKeybindings(string key)
    {
        return keyBindings.KeysActive[key];
    }

    private void OnDestroy() {
        GameObject score = GameObject.FindGameObjectWithTag("ScoreCount");
        score.SendMessage("EndGame");
    }

}
