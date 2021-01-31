using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterSetTime : MonoBehaviour
{
    public float time = 1;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Die", time);
    }

    // Update is called once per frames
    void Update()
    {
        
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
