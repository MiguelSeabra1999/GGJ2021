using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterController : MonoBehaviour
{
    // Start is called before the first frame update

    private Camera m_camera;
    private GameObject postProcessor;
    public bool camera_follow = true;

    private KeyBindings keyBindings;
    private Rigidbody2D rb;
    public float speed;
    public Vector3 jump;
    public float jumpForce = 2.0f;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public float aimSpeed = 15;
    public float dashTime = 0.5f;
    public float dashSpeedup = 1.5f;

    public float midairDirecitonChangeSpeed = 0.3f;

    public float shootingCooldown = 0.3f;
    public float dashingCooldown = 0.3f;

    public Vector3 ModuleOffSet =new Vector3 (1,1,0);
    public float moduleJumpForce = 1;

    public Vector3 dashingDir = Vector3.zero;
    public int jumpDir = 1;


    
    public bool isGrounded = true;
    public bool dashing = false;
    public bool facingForward = true;

    public bool canShoot = true;
    public bool canDash = true;

    public GameObject neckJoint;
    public GameObject armJoint;

    public GameObject GunPoint;

    public GameObject BulletPrefab;
    public GameObject ModulePrefab;
    public float BulletSpawnOffset = 50;
    void Start()
    {
        keyBindings = new KeyBindings(this.gameObject);
        rb = GetComponent<Rigidbody2D>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        m_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        postProcessor = GameObject.Find("PostProcessor");
    }

    // Update is called once per frame
    void Update()
    {
        if(camera_follow)
            m_camera.transform.position = new Vector3 (transform.position.x + 0, transform.position.y + 0, m_camera.transform.position.z); // Camera follows the player with specified offset position
        
        CheckStomp();
        if(CheckGround())
            isGrounded = true;
        else
            isGrounded = false;
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
    }
    private void Shoot()
    {
        Instantiate(BulletPrefab, GunPoint.transform.position, GunPoint.transform.rotation);
        canShoot = false;
        Invoke("RestoreShoot", shootingCooldown);
    }   
    private void Jump()
    {
        if(facingForward)
            jumpDir = 1;
        else    
            jumpDir = -1;
        
         rb.AddForce(jump * jumpForce, ForceMode2D.Impulse);
         isGrounded = false;
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
        this.dashing = true;
        dashingDir = keyBindings.GetLookDir().normalized;
        if(isGrounded)
        {
                if(dashingDir.y <= 0)
                {
                    dashingDir = new Vector3(1,0,0);
                }else if(!facingForward)
                    dashingDir = new Vector3(dashingDir.x*-1,dashingDir.y,0);
  
        }else
        {
           
            if(!facingForward)
                dashingDir = new Vector3(dashingDir.x*-1,dashingDir.y,0);

        }
    
        yield return new WaitForSeconds(dashTime);

        
        this.dashing = false;
        rb.velocity = Vector2.zero;
        Invoke("RestoreDash", dashingCooldown);
    }
    private void RestoreDash()
    {
        canDash = true;
    }

    private void RestoreShoot()
    {
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
        if(dashing)
        {
            SendDamage(other, 0.5f);
            return;
        }
            
        Debug.Log("ouch");
        PostProcessorInterface.DamageEffect(0.4f);
        ModuleType type = keyBindings.RemoveRandomKey() ;
        if(type == ModuleType.EMPTY)
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
                Destroy(obj);
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
        Quaternion targetRotationArm = Quaternion.Euler(0,0,lookAngle + 90);
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
            return true;
        return false;
    }

    private bool CheckGround()
    {
       //return true;
        float floorDist = 1.1f;
        LayerMask mask = LayerMask.GetMask("Floor");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, floorDist, mask);
        Debug.DrawRay(transform.position, -Vector3.up * floorDist,Color.blue);
        if(hit.collider != null && hit.collider.gameObject.layer == 10)
        {
           // Debug.Log(hit.collider.gameObject.name);
            return true;
        }

        return false;
    }
    private bool CheckStomp()
    {
       //return true;
        float floorDist = 1.3f;
        LayerMask mask = LayerMask.GetMask("Enemy");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, floorDist, mask);
        Debug.DrawRay(transform.position, -Vector3.up * floorDist,Color.yellow);
        if(hit.collider != null && hit.collider.gameObject.layer == 9)
        {
            SendDamage(hit.collider.gameObject,1);
            rb.velocity = Vector2.zero;
            Jump();
            return true;
        }

        return false;
    }

}
