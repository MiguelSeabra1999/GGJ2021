using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour
{

    public List<Enemy> EnemyPrefab = new List<Enemy>();

    public List<GameObject> SpawnPositions = new List<GameObject>();
    int last_spawnpoint = 0;

    public int number_of_waves = 5;
    public int enemy_per_wave = 1;

    public enum SpawnType{FIXED_NUMBER, INFINITE_GROWTH};
    public SpawnType type = SpawnType.FIXED_NUMBER;




    public float timeBetweenWaves = 5f;
    public float countDown = 2f;
    public float time_between_enemy_spawns = 1.0f;

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
        if(SpawnPositions.Count==0) SpawnPositions.Add(this.gameObject);
        
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
    private static System.Random rng = new System.Random();

    public  void Shuffle<T>(IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
    

    private Enemy suggestEnemy(float max_cost, out float cost){
        Enemy E = null;
        List<Enemy> newList = new List<Enemy>(EnemyPrefab);
        Shuffle<Enemy>(newList);
        cost = -1;
        foreach(Enemy en in newList){
            if(en && en.spawning_cost<=max_cost) return en;
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
            yield return new WaitForSeconds(time_between_enemy_spawns);
        }


    }

    private void SpawnEnemy(Enemy Enemy)
    {
        int next_spawn_point = (last_spawnpoint+1) % SpawnPositions.Count;
        Debug.Log(next_spawn_point);
        enemies.Add(Instantiate(Enemy, SpawnPositions[next_spawn_point].transform.position, transform.rotation));
        last_spawnpoint = next_spawn_point;
    }
}
