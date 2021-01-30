using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIinterface : MonoBehaviour
{
    public GameObject player;
    public GameObject dodgeIcon;
    public GameObject jumpIcon;
    public GameObject shootIcon;

    private CharacterController playerScript;


    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<CharacterController>();
        Debug.Log("Done");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CheckKey("Shoot", shootIcon);
        CheckKey("Dodge", dodgeIcon);
        CheckKey("Jump" , jumpIcon);
    }

    private void CheckKey( string key, GameObject image)
    {
        bool keyState = playerScript.CheckKeybindings(key);
        if(keyState != image.activeInHierarchy)
        {
            image.SetActive(keyState);
        
        }
    }
}
