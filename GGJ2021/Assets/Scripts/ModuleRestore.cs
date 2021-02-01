using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleRestore : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ModulePrefab;
    public int minTime = 5;
    public int maxTime = 20;
    public GameObject spawnPoint = null;
    void Start()
    {
        StartCoroutine(LifeCycle());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator LifeCycle()
    {
        GameObject newModule;
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minTime,maxTime));
            if(spawnPoint == null)
                newModule = Instantiate(ModulePrefab, transform.position, Quaternion.identity);
            else
                newModule = Instantiate(ModulePrefab, spawnPoint.transform.position, Quaternion.identity);

            newModule.SendMessage("SetRandomModuleType");
            newModule.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }
}
