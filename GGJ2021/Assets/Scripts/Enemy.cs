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

    public float spawning_cost = 1f;


    private CharacterController player;
    private Animator animator;
    public GameObject SmokePrefab;

    // behaviour_type.PATROL
    protected Transform[] points;
    protected int destPoint = 0;
    protected Pathfinding.AIDestinationSetter agent;
    protected Pathfinding.AILerp ai_lerp;
    public GameObject Path_list_root;

    


    // behaviour_type.ALWAYS_FORWARD
    private bool hasLandedFirstTime = false;
    // behaviour_type.TURRET

    public bool unlocked = true;
    public float vision_range = 100f;
    private Vector3 turret_target;
    private float laser_start_time;
    public float laser_durantion = 10f;
    public float laser_charge_time = 5f;
    private float shootingCooldown = 6f;
    private float last_shoot_time;
    private bool canShoot = true;
    public Lightbug.LaserMachine.LaserMachine laser;
    Vector3 vecToobj_turret;
    public AudioSource turret_sound_1;
    public bool turret_flip=false;

    public GameObject EnemyBulletPrefab;

    private Rigidbody2D rb;

    //Audio
    [FMODUnity.EventRef]
    public string enemyFire = string.Empty;
    [FMODUnity.EventRef]
    public string enemyHit = string.Empty;

    void Start () {

        rb = GetComponent<Rigidbody2D>();
        GameObject P = GameObject.FindGameObjectWithTag("Player");
        if(P){
            P.BroadcastMessage("AddMe", gameObject);
            player = P.GetComponent<CharacterController>();
        }

        animator = GetComponent<Animator>();
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
            // Random patrol path
            if(Path_list_root==null){
                List<GameObject> list = new List<GameObject>();
                list.Add(GameObject.Find("Patrol_variant_1"));
                list.Add(GameObject.Find("Patrol_variant_2"));
                list = Fisher_Yates_CardDeck_Shuffle(list);
                if(list[0]!=null){
                    Path_list_root = list[0];
                    if(Path_list_root != null){
                        points = new Transform[Path_list_root.transform.childCount];
                        for(int i =0; i<Path_list_root.transform.childCount; i++){
                            points[i] = Path_list_root.transform.GetChild(i);
                        }
                    }

                }
            }
            GotoNextPoint();
            GetComponentInChildren<DroneSpriteInterface>().Init();
        }

        if(Random.Range(0f,1f) > 0.5f)
            Flip();
        laser_start_time = Time.realtimeSinceStartup-laser_durantion;
        last_shoot_time = Time.realtimeSinceStartup-shootingCooldown;
        if(behaviour == behaviour_type.TURRET)
            laser.m_active = false;
    }

     public static List<GameObject> Fisher_Yates_CardDeck_Shuffle (List<GameObject>aList) {
 
         System.Random _random = new System.Random ();
 
         GameObject myGO;
 
         int n = aList.Count;
         for (int i = 0; i < n; i++)
         {
             // NextDouble returns a random number between 0 and 1.
             // ... It is equivalent to Math.random() in Java.
             int r = i + (int)(_random.NextDouble() * (n - i));
             myGO = aList[r];
             aList[r] = aList[i];
             aList[i] = myGO;
         }
 
         return aList;
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

    public void resetPatrolPath(Vector3 pos, bool teleport){

        attacking_player = false;

        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        destPoint = 0;
        // Set the agent to go to the currently selected destination.
        agent.target = points[destPoint];

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
        if(teleport)
            ai_lerp.Teleport(pos);
    }

    private void RestoreShoot()
    {
        canShoot = true;
    }

    void FixedUpdate () {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if(HP<=0)
        {
           // if()
            return;
        }
            
        if (behaviour == behaviour_type.PATROL){
            

            ai_lerp.speed = speed;
            if(agent.target != null && Vector3.Distance(transform.position, agent.target.position) < 2f){
                GotoNextPoint();
            }
            if(player){
                var obj = player.transform;
                if (unlocked && Vector3.Distance(player.transform.position, transform.position) < vision_range)
                {
                    Vector3 vecToobj = new Vector3(obj.position.x - this.transform.position.x, obj.position.y - this.transform.position.y, obj.position.z - this.transform.position.z);
                    //Debug.DrawLine(obj.position, this.transform.position, Color.red, 3f);
                    float angle = Vector3.Angle(transform.forward, vecToobj);
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, vecToobj,vision_range, ~(LayerMask.GetMask("Enemy","Bullet","Enemy_Bullet")));
                    //Debug.DrawRay(transform.position, vecToobj, Color.green);
                    if (hit && hit.collider != null && hit.collider.tag == player.tag)
                    {
                        Vector3 myLocation = transform.position;
                        Vector3 targetLocation = obj.position;
                        targetLocation.z = myLocation.z; // ensure there is no 3D rotation by aligning Z position
                        
                        // vector from this object towards the target location
                        Vector3 vectorToTarget = targetLocation - myLocation;
                        // rotate that vector by 90 degrees around the Z axis
                        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
                        
                        // get the rotation that points the Z axis forward, and the Y axis 90 degrees away from the target
                        // (resulting in the X axis facing the target)
                        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);
                                            
                        // changed this from a lerp to a RotateTowards because you were supplying a "speed" not an interpolation value
                        //transform.rotation = targetRotation;//Quaternion.RotateTowards(transform.rotation, targetRotation, 5 * Time.deltaTime);

                        Debug.DrawLine(transform.position, player.transform.position, Color.red);
                        if(canShoot){ // &&  (Time.realtimeSinceStartup-last_shoot_time)> shootingCooldown
                            Instantiate(EnemyBulletPrefab, transform.position, targetRotation * Quaternion.Euler(0, 0, 90));
                            //last_shoot_time = Time.realtimeSinceStartup;
                            canShoot = false;
                            Invoke("RestoreShoot", shootingCooldown);
            
                            //Audio
                            FMODUnity.RuntimeManager.PlayOneShot(enemyFire);
                        }
                        
                    }

                }
            } else{
                //transform.rotation = new Quaternion();
            }
        } else 
        if (behaviour == behaviour_type.ALWAYS_FORWARD){
            
            bool grounded = CheckGround();
            if( grounded != animator.GetBool("Grounded"))
            {
                animator.SetBool("Grounded", grounded);
            }
            if(grounded)
            {
                if(!hasLandedFirstTime)
                    hasLandedFirstTime = true;
                transform.Translate(new Vector3(speed/10,0,0));
            }
            else if(hasLandedFirstTime)
                transform.Translate(new Vector3(speed/20,0,0));

        } else
        if (behaviour == behaviour_type.TURRET){
            if(player){
                var obj = player.transform;
                if (unlocked && (Time.realtimeSinceStartup-laser_start_time)> laser_durantion && Vector3.Distance(player.transform.position, transform.position) < vision_range)
                {

                    animator.SetBool("Player_in_sight", true);
                    Vector3 vecToobj = new Vector3(obj.position.x - this.transform.GetChild(0).position.x, obj.position.y - this.transform.GetChild(0).position.y, obj.position.z - this.transform.GetChild(0).position.z);
                    //Debug.DrawLine(obj.position, this.transform.position, Color.red, 3f);
                    float angle = Vector3.Angle(Vector3.right, vecToobj);
                    //Vector3 vecToobj_norm = vecToobj.normalized;
                    
                    //Debug.Log(vecToobj);
                    //Debug.Log(angle);
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.GetChild(0).position, vecToobj, vision_range, ~(LayerMask.GetMask("Enemy","Bullet","Enemy_Bullet")));
                    //Debug.DrawRay(transform.position, vecToobj, Color.green);
                    if (hit && hit.collider != null && hit.collider.tag == player.tag)
                    {
                        Vector3 myLocation = this.transform.GetChild(0).position;
                        Vector3 targetLocation = obj.position;
                        targetLocation.z = myLocation.z; // ensure there is no 3D rotation by aligning Z position
                        
                        // vector from this object towards the target location
                        Vector3 vectorToTarget = targetLocation - myLocation;
                        // rotate that vector by 90 degrees around the Z axis
                        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, -90) * vectorToTarget;
                        
                        // get the rotation that points the Z axis forward, and the Y axis 90 degrees away from the target
                        // (resulting in the X axis facing the target)
                        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);
                        
                        // changed this from a lerp to a RotateTowards because you were supplying a "speed" not an interpolation value
                        transform.GetChild(0).rotation = targetRotation;//Quaternion.RotateTowards(transform.rotation, targetRotation, 5 * Time.deltaTime);
                        if(turret_flip){
                            transform.GetChild(0).Rotate(new Vector3(180,0,0), Space.Self);
                        }


                        Debug.DrawLine(this.transform.GetChild(0).position, player.transform.position + vecToobj*10, Color.red, laser_durantion);
                        laser_start_time = Time.realtimeSinceStartup;
                        
                        laser.enabled=true;
                        laser.m_active = true;
                        laser.seconds_to_disable = laser_durantion;
                        laser.turnoff_timer = true;
                        laser.reset_turn_off_timer();
                        
                        laser.m_currentProperties.m_intermittent = true;
                        laser.m_currentProperties.m_intervalTime = 0.5f;
                        laser.change_width(0, 0.1f);

                        unlocked = false;
                        vecToobj_turret = vecToobj;
                        animator.SetTrigger("Charge");
                        this.StartCoroutine(laserDmg());
                        //Invoke("chargedLaser",laser_charge_time);
                        //Invoke("disableLaser",laser_durantion);
                        
                    }
                } else {
                    animator.SetBool("Player_in_sight", false);
                }
                if((Time.realtimeSinceStartup-laser_start_time) > laser_durantion){
                    //transform.rotation = new Quaternion();
                    //laser.m_active = false;
                    //laser.m_currentProperties.m_intermittent = false;
                }
            }
        }
    }


    public void disableLaser(){
        laser.m_currentProperties.m_intermittent = false;
        laser.m_active = false;
        laser.enabled=false;
        unlocked = true;
        Debug.Log("disabled turret");
        turret_sound_1.enabled = false;
    }

    public void chargedLaser(){
        laser.m_currentProperties.m_intermittent = false;
        laser.m_active = true;
        laser.change_width(0, 0.35f);
        //player.gameObject.SendMessage("Damage");
        Debug.Log("laser charged");
        turret_sound_1.enabled = true;

    }

    public IEnumerator laserDmg(){
        yield return new WaitForSeconds(laser_charge_time);
        chargedLaser();
        float start_time = Time.realtimeSinceStartup;
        while (!unlocked && (Time.realtimeSinceStartup-start_time) < (laser_durantion-laser_charge_time)){
            Debug.Log("Laser tentative");
            RaycastHit2D hit = Physics2D.Raycast(this.transform.GetChild(0).position, vecToobj_turret, vision_range, ~(LayerMask.GetMask("Enemy","Bullet","Enemy_Bullet")));
            //Debug.DrawRay(transform.position, vecToobj, Color.green);
            if (hit && hit.collider != null && player && hit.collider.tag == player.tag)
            {
                hit.collider.gameObject.SendMessage("Damage_null", this.gameObject);
                Debug.Log("damaging player");
            }
            yield return new WaitForSeconds(0.1f);
        }
        disableLaser();
    }


    private void Flip()
    {
   //  Debug.Log("Flip");

        if(behaviour == behaviour_type.TURRET){
        
            // if(transform.localRotation.eulerAngles.y == 0 )
            // {
            //     transform.localRotation =  Quaternion.Euler(0,180,0); 
            
            // }
            // else 
            // {
            //     transform.localRotation =  Quaternion.Euler(0,0,0); 
            
            // }

            return;
        }


        if(transform.rotation.eulerAngles.y == 0 )
        {
            transform.rotation =  Quaternion.Euler(0,180,0); 
          
        }
        else 
        {
            transform.rotation =  Quaternion.Euler(0,0,0); 
         
        }
    }

    private void Stomped()
    {
        
        if(behaviour == behaviour_type.ALWAYS_FORWARD || behaviour ==behaviour_type.PATROL)
        {
            gameObject.layer = 13;
            if(behaviour ==behaviour_type.PATROL)
            {
                  ai_lerp.speed = 0;
                  ai_lerp.enabled = false;
                  agent.enabled = false;
                transform.rotation = Quaternion.Euler(0,0,0);
                
            }

            animator.SetTrigger("Stomp");
            Invoke("Die",0.7f);
        }
    }

    private void GetShot()
    {
   
        Instantiate(SmokePrefab, transform.position, Quaternion.identity);
        FMODUnity.RuntimeManager.PlayOneShot(enemyHit);
        Die();
        //Audio
    }

    private void Die()
    {
        Instantiate(SmokePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private bool CheckGround()
    {
       //return true;
        float floorDist = 0.6f;
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

    private void OnCollisionEnter2D(Collision2D other) {
        if(behaviour == behaviour_type.ALWAYS_FORWARD
        && (other.collider.gameObject.layer == 8 || other.collider.gameObject.layer == 7)
        ){
            Flip();
        }

        if(other.collider.gameObject.layer == 7 && HP > 0)
            other.collider.gameObject.SendMessage("Damage", gameObject);
        
    }

    private void OnDestroy() {
        GameObject score = GameObject.FindGameObjectWithTag("ScoreCount");
        if(score == null)return;
        UnityEngine.UI.Text txt = score.GetComponent<UnityEngine.UI.Text>(); 
        txt.text = (int.Parse(txt.text) + 1).ToString();
    }

}
