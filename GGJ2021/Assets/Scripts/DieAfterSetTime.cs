using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterSetTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Die", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
