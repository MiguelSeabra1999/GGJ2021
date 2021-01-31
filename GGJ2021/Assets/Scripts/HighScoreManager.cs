using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;

public class HighScoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    string path;
    UnityEngine.UI.Text txt;
    UnityEngine.UI.Text hightxt;
    GameObject hscore ;
    private int currentMax = 0 ;
    GameObject score;
    public GameObject turnOnAtEnd;
    void Start()
    {
        path = "Assets/dontOpenOrEditThatWouldBeCheating.txt";
        score = GameObject.FindGameObjectWithTag("ScoreCount");
        UnityEngine.UI.Text txt = score.GetComponent<UnityEngine.UI.Text>(); 
        hscore = GameObject.FindGameObjectWithTag("HighScoreText");
        UnityEngine.UI.Text hightxt = hscore.GetComponent<UnityEngine.UI.Text>(); 
        //txt.text = (int.Parse(txt.text) + 1).ToString();
        if(File.Exists(path))
        {
            StreamReader reader = new StreamReader(path); 
            currentMax = int.Parse(reader.ReadToEnd());
            reader.Close();
            hightxt.text = "High Score: " + currentMax;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EndGame()
    {
        UnityEngine.UI.Text txt = score.GetComponent<UnityEngine.UI.Text>(); 
        UnityEngine.UI.Text hightxt = hscore.GetComponent<UnityEngine.UI.Text>(); 
        if(int.Parse(txt.text) > currentMax)
        {
            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(txt.text);
            writer.Close(); 

            hightxt.text = "High Score: " + int.Parse(txt.text);
        }
        turnOnAtEnd.SetActive(true);
    }
}
