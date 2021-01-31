using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cave_transporter : MonoBehaviour
{

    public Transform destination;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.layer==LayerMask.NameToLayer("Enemy")){
            other.transform.position = destination.position;
        }

    }
}
