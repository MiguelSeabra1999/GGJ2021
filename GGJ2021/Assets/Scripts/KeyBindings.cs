using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class KeyBindings
    {
        public Dictionary<string, bool> Keys = new Dictionary<string, bool>() {
            {"Shoot", false },
            {"Dodge", false },
            {"Jump", false }
        };

        public Dictionary<string, bool> KeysActive = new Dictionary<string, bool>() {
            {"Shoot", true },
            {"Dodge", true },
            {"Jump", true }
        };


        private Dictionary<string, GameObject> KeysObjs = new Dictionary<string, GameObject>() {
            {"Shoot", GameObject.Find("Key_Left_Click") },
            {"Dodge", GameObject.Find("Key_Shift")},
            {"Jump", GameObject.Find("Key_Space")}
        };
 



        public void UpdateKeys()
        {
            // FIXME movement keys are unchangeable(forced as wasd and arrows); mousewheel scrolls are not supported
            Keys["Shoot"] = Input.GetKeyDown(KeyCode.Mouse0) && KeysActive["Shoot"];
            Keys["Dodge"] = Input.GetKeyDown(KeyCode.LeftShift) && KeysActive["Dodge"];
            Keys["Jump"] = Input.GetKeyDown(KeyCode.Space) && KeysActive["Jump"];

            foreach(string k in KeysActive.Keys){
                KeysObjs[k].SetActive(KeysActive[k]);
            }

        }

        public Vector3 GetDirection() //gets the direction the player is inputing
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            //float vertical = Input.GetAxisRaw("Vertical");
            return new Vector3(horizontal, 0f, 0f).normalized;
        }


    }
