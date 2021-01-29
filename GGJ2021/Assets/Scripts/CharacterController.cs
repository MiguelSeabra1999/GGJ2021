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
    
    public bool isGrounded = true;
    void Start()
    {
        keyBindings = new KeyBindings();
        rb = GetComponent<Rigidbody2D>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        keyBindings.UpdateKeys();
        Vector3 dir = keyBindings.GetDirection() *speed * Time.deltaTime;
        if(dir.magnitude > 0.01f)
            this.transform.Translate(dir.x  ,0,0);
        if(keyBindings.Keys["Jump"] && isGrounded)
            Jump();
        

        
    }

    private void Jump()
    {
         rb.AddForce(jump * jumpForce, ForceMode2D.Impulse);
         isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        isGrounded = true;
    }

}
