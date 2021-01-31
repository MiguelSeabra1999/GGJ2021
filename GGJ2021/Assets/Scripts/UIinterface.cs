using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIinterface : MonoBehaviour
{
    public GameObject player;
    public GameObject dodgeIcon;
    public GameObject jumpIcon;
    public GameObject shootIcon;

    public GameObject dodgeIcon2;
    public GameObject jumpIcon2;
    public GameObject shootIcon2;

    private CharacterController playerScript;


    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<CharacterController>();
//        Debug.Log("Done");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CheckKey("Shoot", shootIcon, shootIcon2);
        CheckKey("Dodge", dodgeIcon, dodgeIcon2);
        CheckKey("Jump" , jumpIcon, jumpIcon2);
    }

    private void CheckKey( string key, GameObject image , GameObject image2)
    {
        bool keyState = playerScript.CheckKeybindings(key);
        if(keyState != image.activeInHierarchy)
        {
            image.SetActive(keyState);
            image2.SetActive(!keyState);
        
        }
    }
}
