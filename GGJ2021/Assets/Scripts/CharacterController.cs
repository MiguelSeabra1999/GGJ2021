using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterController : MonoBehaviour
{
    // Start is called before the first frame update



    private KeyBindings keyBindings;
    private Rigidbody2D rb;
    public float speed;
    public Vector3 jump;
    public float jumpForce = 2.0f;

    public float aimSpeed = 15;
    
    public bool isGrounded = true;
    public bool facingForward = true;

    public GameObject neckJoint;
    public GameObject armJoint;

    public GameObject GunPoint;

    public GameObject BulletPrefab;
    public float BulletSpawnOffset = 50;
    void Start()
    {
        keyBindings = new KeyBindings(this.gameObject);
        rb = GetComponent<Rigidbody2D>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        keyBindings.UpdateKeys();
        (float, bool) LookInfo = keyBindings.GetLookAngle();
        float lookAngle = LookInfo.Item1;
        bool lookingBack = LookInfo.Item2;
        Vector3 dir = keyBindings.GetDirection() *speed * Time.deltaTime;
        if(dir.magnitude > 0.01f)
            if(facingForward)
                this.transform.Translate(dir.x  ,0,0);
            else
                this.transform.Translate(-1*dir.x  ,0,0);
        if(keyBindings.Keys["Jump"] && isGrounded)
            Jump();
        if(keyBindings.Keys["Shoot"])
            Shoot();
        UpdateLookDirection(lookAngle, lookingBack);
    }   

    private void Shoot()
    {
        Instantiate(BulletPrefab, GunPoint.transform.position, GunPoint.transform.rotation);
    }   
    private void Jump()
    {
         rb.AddForce(jump * jumpForce, ForceMode2D.Impulse);
         isGrounded = false;
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
    private void OnCollisionEnter2D(Collision2D other) 
    {
        isGrounded = true;
    }

}
