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

    public float HP = 1f;

    private CharacterController player;



    // behaviour_type.PATROL
    protected Transform[] points;
    protected int destPoint = 0;
    protected Pathfinding.AIDestinationSetter agent;
    protected Pathfinding.AILerp ai_lerp;
    public GameObject Path_list_root;


    // behaviour_type.ALWAYS_FORWARD

    // behaviour_type.TURRET

    public bool active = true;
    public float vision_range = 100f;
    private Vector3 turret_target;
    private float laser_start_time;
    public float laser_durantion = 10f;


    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
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
        laser_start_time = Time.realtimeSinceStartup-laser_durantion;
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
            var obj = player.transform;
            if (active && Vector3.Distance(player.transform.position, transform.position) < vision_range)
            {
                Vector3 vecToobj = new Vector3(obj.position.x - this.transform.position.x, obj.position.y - this.transform.position.y, obj.position.z - this.transform.position.z);
                //Debug.DrawLine(obj.position, this.transform.position, Color.red, 3f);
                float angle = Vector3.Angle(transform.forward, vecToobj);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, vecToobj, 500, 7);
                //Debug.DrawRay(transform.position, vecToobj, Color.green);
                if (hit && hit.collider != null)
                {
                    transform.Rotate(new Vector3(0,0,angle));
                    Debug.DrawLine(transform.position, player.transform.position, Color.red);
                    //TODO: instantiate EnemyBullet and deal damage to player
                }
            } else{
                transform.rotation = new Quaternion();
            }
        } else 
        if (behaviour == behaviour_type.ALWAYS_FORWARD){
            transform.Translate(new Vector3(speed/10,0,0));
        } else
        if (behaviour == behaviour_type.TURRET){
            var obj = player.transform;
            if (active && (Time.realtimeSinceStartup-laser_start_time)> laser_durantion && Vector3.Distance(player.transform.position, transform.position) < vision_range)
            {
                Vector3 vecToobj = new Vector3(obj.position.x - this.transform.position.x, obj.position.y - this.transform.position.y, obj.position.z - this.transform.position.z);
                //Debug.DrawLine(obj.position, this.transform.position, Color.red, 3f);
                float angle = Vector3.Angle(transform.forward, vecToobj);
                
                RaycastHit2D hit = Physics2D.Raycast(transform.position, vecToobj,500, LayerMask.NameToLayer("Player"));
                //Debug.DrawRay(transform.position, vecToobj, Color.green);
                if (hit && hit.collider != null)
                {
                    transform.rotation = new Quaternion();
                    transform.Rotate(new Vector3(0,0,angle));
                    Debug.DrawLine(transform.position, player.transform.position + vecToobj*10, Color.red, laser_durantion);
                    laser_start_time = Time.realtimeSinceStartup;
                    //TODO: instantiate laser and deal damage to player
                }
            } else if((Time.realtimeSinceStartup-laser_start_time)> laser_durantion){
                transform.rotation = new Quaternion();
            }
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
