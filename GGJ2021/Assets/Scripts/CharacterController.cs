using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;



public class CharacterController : MonoBehaviour
{
    // Start is called before the first frame update

    private Camera m_camera;
    public bool camera_follow = true;

    private KeyBindings keyBindings;
    private Rigidbody2D rb;
    public float speed;
    public Vector3 jump;
    public float jumpForce = 2.0f;

    public float aimSpeed = 15;
    public float dashTime = 0.5f;
    public float dashSpeedup = 1.5f;

    public float midairDirecitonChangeSpeed = 0.3f;

    public int dashingDir = 1;
    public int jumpDir = 1;
    
    public bool isGrounded = true;
    public bool dashing = false;
    public bool facingForward = true;

    public GameObject neckJoint;
    public GameObject armJoint;

    public GameObject GunPoint;

    public GameObject BulletPrefab;
    public float BulletSpawnOffset = 50;
<<<<<<< Updated upstream
=======

    //Audio
    [FMODUnity.EventRef]
    public string jumpSFX = string.Empty;
    [FMODUnity.EventRef]
    public string landSFX = string.Empty;
    [FMODUnity.EventRef]
    public string walkSFX = string.Empty;

    [HideInInspector]public SpriteRenderer[] sprites;
>>>>>>> Stashed changes
    void Start()
    {
        keyBindings = new KeyBindings(this.gameObject);
        rb = GetComponent<Rigidbody2D>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        m_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(camera_follow)
            m_camera.transform.position = new Vector3 (transform.position.x + 0, transform.position.y + 0, m_camera.transform.position.z); // Camera follows the player with specified offset position
        
        keyBindings.UpdateKeys();
        (float, bool) LookInfo = keyBindings.GetLookAngle();
        float lookAngle = LookInfo.Item1;
        bool lookingBack = LookInfo.Item2;
        Vector3 dir = keyBindings.GetDirection() *speed * Time.deltaTime;

        if(dashing)
        {
            UpdateDash();
            return;
        }

        
        if(dir.magnitude > 0.01f)
            UpdateMovement(dir);



        if(keyBindings.Keys["Jump"] && isGrounded)
            Jump();
<<<<<<< Updated upstream
        if(keyBindings.Keys["Shoot"])
=======
        if (keyBindings.Keys["Shoot"] && canShoot)
>>>>>>> Stashed changes
            Shoot();
        if(keyBindings.Keys["Dodge"])
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
    }   
    private void Jump()
    {
        if(facingForward)
            jumpDir = 1;
        else    
            jumpDir = -1;
        
         rb.AddForce(jump * jumpForce, ForceMode2D.Impulse);
         isGrounded = false;

        //Audio
        FMODUnity.RuntimeManager.PlayOneShot(jumpSFX);
    }
<<<<<<< Updated upstream
=======
    private void Jump(float mod)
    {
        if(facingForward)
            jumpDir = 1;
        else    
            jumpDir = -1;
        
         rb.AddForce(jump * jumpForce * mod, ForceMode2D.Impulse);
         isGrounded = false;
        //Audio
        FMODUnity.RuntimeManager.PlayOneShot(jumpSFX);
    }
>>>>>>> Stashed changes

    private void Dash()
    {
      //  Debug.Log("DashStart");
        this.StartCoroutine(Dashing());
    }

    IEnumerator Dashing()
    {
  //      Debug.Log("coroutine");
        this.dashing = true;

            dashingDir = 1;
    
        yield return new WaitForSeconds(dashTime);
        this.dashing = false;
    }

    private void UpdateDash()
    {
      //  Debug.Log("update");
        this.transform.Translate(dashingDir * speed * Time.deltaTime * dashSpeedup,0,0);
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
        //Audio
        FMODUnity.RuntimeManager.PlayOneShot(landSFX);
    }

    private void TouchWall()
    {
        dashing = false;
        
    }


    private void OnCollisionEnter2D(Collision2D col) 
    {
        
        
            if(col.otherCollider.gameObject.name == "Feet")
                if(col.gameObject.layer ==10)
                    TouchGround();
            if(col.gameObject.layer ==8)
                TouchWall();
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

}
