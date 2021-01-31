using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpriteInterface : MonoBehaviour
{
    // Start is called before the first frame update
    private Enemy brain;
    private GameObject player;
    private bool dead = false;
    private bool inverted = false;
    private Animator animator;
    public void Init()
    {
        animator = GetComponent<Animator>();
        brain = GetComponentInParent<Enemy>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
       // if(dead )return;
        this.transform.rotation = Quaternion.identity;
        if(player == null)return;
        if(player.transform.position.x <transform.position.x && !inverted )
        {
            animator.SetTrigger("TurnAround");
            Debug.Log("trun");
            inverted = true;

        }else if(player.transform.position.x >transform.position.x && inverted )
        {
            animator.SetTrigger("TurnAround");
            Debug.Log("trun1");
            inverted = false;  
        }
        if(player.transform.position.x >transform.position.x )
        {
            Flip();

        }


        if(brain.HP <= 0 && !dead)
        {
            animator.SetTrigger("Stomp");
            dead = true;
        }

    }

    private void Flip()
    {
        //animator.SetTrigger("TurnAround");
        if(transform.rotation.eulerAngles.y == 0 )
        {
            transform.rotation =  Quaternion.Euler(0,180,0); 
         
        }
        else 
        {
            transform.rotation =  Quaternion.Euler(0,0,0); 
            
         
        }
    }
}
