using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    private float speed = 10;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(0,-1*speed*Time.deltaTime,0);
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {

         if(other.gameObject.layer!=7)
        Destroy(gameObject);    
    }
  /*  private void OnCollisionEnter2D(Collision other) {
        Debug.Log("Boom");
        Destroy(gameObject);
    }*/
}