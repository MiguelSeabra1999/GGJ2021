using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour
{

    public Enemy EnemyPrefab;

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

            if(!wait_for_wave_clear){
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

    private IEnumerator SpawnWave()
    {   
        if(type == SpawnType.FIXED_NUMBER){
            for (int i = 0; i < enemy_per_wave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(0.5f);
            }
        } else
        if(type == SpawnType.INFINITE_GROWTH) {
            waveIndex++;
            for (int i = 0; i < waveIndex; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(0.5f);
            }
        }

    }

    private void SpawnEnemy()
    {
        enemies.Add(Instantiate(EnemyPrefab, transform.position, transform.rotation));
    }
}
