using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Pathfinding.Seeker))]
[RequireComponent(typeof(Pathfinding.AILerp))]
[RequireComponent(typeof(Pathfinding.AIDestinationSetter))]
public class Enemy : MonoBehaviour
{
    public  enum behaviour_type {ALWAYS_FORWARD, PATROL, TURRET};

    public Enemy.behaviour_type behaviour = behaviour_type.ALWAYS_FORWARD;

    public bool attacking_player=false;
    public float speed = 3f;

    public float HP = 100f;



    // behaviour_type.PATROL
    protected Transform[] points;
    protected int destPoint = 0;
    protected Pathfinding.AIDestinationSetter agent;
    protected Pathfinding.AILerp ai_lerp;
    public GameObject Path_list_root;


    // behaviour_type.ALWAYS_FORWARD


    void Start () {

        // we always setup if there is points defined, so we can change behaviour at runtime if needed
        agent = GetComponent<Pathfinding.AIDestinationSetter>();
        ai_lerp = GetComponent<Pathfinding.AILerp>();
        ai_lerp.speed = speed;
        if(Path_list_root != null){
            points = new Transform[Path_list_root.transform.childCount];
            for(int i =0; i<Path_list_root.transform.childCount; i++){
                points[i] = Path_list_root.transform.GetChild(i);
            }
        }

        if(behaviour == behaviour_type.PATROL){
            GotoNextPoint();
        }
    }


    // Only if behaviour_type.PATROL
    void GotoNextPoint() {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        if(attacking_player){
            agent.target = null;
            return;
        }
        // Set the agent to go to the currently selected destination.
        agent.target = points[destPoint];

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }


    void FixedUpdate () {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (behaviour == behaviour_type.PATROL){
            ai_lerp.speed = speed;
            if(agent.target != null && Vector3.Distance(transform.position, agent.target.position) < 2f){
                GotoNextPoint();
            }
        } else 
        if (behaviour == behaviour_type.ALWAYS_FORWARD){
            transform.Translate(new Vector3(speed/10,0,0));
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(behaviour == behaviour_type.ALWAYS_FORWARD
        && other.collider.gameObject.layer == 8
        ){
            speed = speed*-1;
        }
        
    }

}
