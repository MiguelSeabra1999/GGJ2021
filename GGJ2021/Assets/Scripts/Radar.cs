using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    // Start is called before the first frame update

    public List<(GameObject,bool)> targets = new List<(GameObject,bool)>();
    public float MAXDIST = 5;
    public float MINDIST = 2;
    public GameObject ArrowPrefab;
    public GameObject ArrowPrefab1;
    
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        MonitorTargets();
    }

    private void AddMe(GameObject obj)
    {
       // Debug.Log("AddingPossibleTarget");
        targets.Add((obj,false));
    }



    private void MonitorTargets()
    {
        int j = 0;
        for(int i = 0; i < targets.Count; i++)
        {
            if(targets[j].Item1 == null)
            {
                targets.RemoveAt(j);
              
            }else
            {
                j++;
            }
        }
        for(int i = 0; i < targets.Count; i++)
        {
            float mag = (targets[i].Item1.transform.position - transform.position).magnitude;
            if(mag >= MINDIST && mag <= MAXDIST && !targets[i].Item2)
            {
                CreateArrow(targets[i].Item1);
                targets[i] = (targets[i].Item1,true);
            }
        }
    }

    private void CreateArrow(GameObject target)
    {
        Debug.Log("arrow");
        GameObject newArrow = null;
        if(target.CompareTag("Enemy"))
            newArrow = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
        else
            newArrow = Instantiate(ArrowPrefab1, transform.position, Quaternion.identity);
        newArrow.transform.parent = transform;
        RadarArrow script = newArrow.GetComponent<RadarArrow>();
        script.MAXDIST = MAXDIST;
        script.MINDIST = MINDIST;
        script.target = target;
    //    target.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
      //  target.GetComponent<SpriteRenderer>().sortingOrder  = 1;
        if(target.CompareTag("Enemy"))
            newArrow.GetComponent<Renderer>().material.color =  new Color(1, 0 ,0.086f, 0.4f);
        else   
        {
            Module mod = target.GetComponent<Module>();
           // target.GetComponent<SpriteRenderer>().sortingOrder  = 0;
            switch(mod.type)
            {
                case ModuleType.DASH:
                    newArrow.GetComponent<Renderer>().material.color = new Color(1, 0.38f ,0.89f, 0.8f);
                return;
                case ModuleType.JUMP:
                    newArrow.GetComponent<Renderer>().material.color = new Color(0, 0.92f ,0.68f, 0.8f);
                return;
                case ModuleType.SHOOT:
                    newArrow.GetComponent<Renderer>().material.color = new Color(0, 0.87f ,0.95f, 0.8f);
                return;
            }
            
        }

    }

    private void RemoveTracking(GameObject obj)
    {
        for(int i = 0; i < targets.Count; i++)
        {
    
            if(targets[i].Item1 == obj)
            {
           
                targets[i] = (targets[i].Item1,false);
            }
        }
    }
}
