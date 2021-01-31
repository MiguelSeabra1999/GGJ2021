using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // Start is called before the first frame update
    private float speed = 3;
    private float damage = 1;
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
        if(other.gameObject.layer==LayerMask.NameToLayer("Player")){
            other.gameObject.SendMessage("Damage_null", this.gameObject);
        }
        Destroy(this.gameObject);

    }
}
