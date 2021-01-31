using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameRestarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        RestartGame();   
    }

    public void RestartGame()
    {
        Debug.Log("Restarting");
        SceneManager.LoadScene(0);
    }
}
