using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour
{

    public List<Enemy> EnemyPrefab = new List<Enemy>();

    public List<GameObject> SpawnPositions = new List<GameObject>();

    public int number_of_waves = 5;
    public int enemy_per_wave = 1;

    public enum SpawnType{FIXED_NUMBER, INFINITE_GROWTH};
    public SpawnType type = SpawnType.FIXED_NUMBER;




    public float timeBetweenWaves = 5f;
    public float countDown = 2f;

    //public float searchCountdown = 1f; 

    private List<Enemy> enemies = new List<Enemy>();
    private int waveIndex = 0;

    public enum SpawnState{SPAWNING, WAITING, COUNTING};
    public SpawnState state = SpawnState.WAITING;
    private bool initiate_stop = false;

    public bool activate_on_start = false;

    public bool wait_for_wave_clear = false;



    // Start is called before the first frame update
    void Start()
    {
        if(activate_on_start) activate();
        
    }

    public void activate(){
        StartCoroutine(RunSpawner());
    }

    public void deactivate(){
        initiate_stop = true;
    }

    // Update is called once per frame
    void Update()
    {
    }



    // this replaces your Update method
    private IEnumerator RunSpawner()
    {        
        // first time wait 2 seconds
        yield return new WaitForSeconds(countDown);

        // run this routine infinite
        while(!initiate_stop)
        {
            state = SpawnState.SPAWNING;    

            // do the spawning and at the same time wait until it's finished
            yield return SpawnWave();

            state = SpawnState.WAITING;

            if(wait_for_wave_clear){
                // wait until all enemies died (are destroyed)
                yield return new WaitWhile(EnemyisAlive);
            }

            state = SpawnState.COUNTING;

            // wait 5 seconds
            yield return new WaitForSeconds(timeBetweenWaves);
        }
        initiate_stop = false;
    }

    private bool EnemyisAlive()
    {        
        // uses Linq to filter out null (previously detroyed) entries
        enemies = enemies.Where(e => e != null).ToList();

        return enemies.Count > 0;
    }

     public static List<Enemy> Fisher_Yates_CardDeck_Shuffle (List<Enemy>aList) {
 
         System.Random _random = new System.Random ();
 
         Enemy myGO;
 
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
    

    private Enemy suggestEnemy(float max_cost, out float cost){
        Enemy E = null;
        List<Enemy> newList = Fisher_Yates_CardDeck_Shuffle(new List<Enemy>(EnemyPrefab));
        cost = -1;
        foreach(Enemy en in newList){
            if(en.spawning_cost<=max_cost) return en;
        }

        return E;
    }


    private List<Enemy> Wave()
    {   
        List<Enemy> enemies_to_spawn = new List<Enemy>();


        if(type == SpawnType.FIXED_NUMBER){
            for (int i = 0; i < enemy_per_wave; i++)
            {
                enemies_to_spawn.Add(EnemyPrefab[0]);
            }
        } else
        if(type == SpawnType.INFINITE_GROWTH) {
            float max_cost =waveIndex;
            for (int i = 0; i < waveIndex; i++)
            {
                float cost;
                Enemy en = suggestEnemy(max_cost, out cost);
                if(en){
                    enemies_to_spawn.Add(en);
                    max_cost-=cost;

                }
            }
        }
        return enemies_to_spawn;
    }


    private IEnumerator SpawnWave()
    {   
        if(type == SpawnType.INFINITE_GROWTH) {
            waveIndex++;
        }

        List<Enemy> enemies_to_spawn = Wave();

        foreach (Enemy e in enemies_to_spawn)
        {
            SpawnEnemy(e);
            yield return new WaitForSeconds(0.5f);
        }


    }

    private void SpawnEnemy(Enemy Enemy)
    {
        enemies.Add(Instantiate(Enemy, transform.position, transform.rotation));
    }
}
