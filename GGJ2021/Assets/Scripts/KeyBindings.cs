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



        public void UpdateKeys()
        {
            // FIXME movement keys are unchangeable(forced as wasd and arrows); mousewheel scrolls are not supported
            Keys["Shoot"] = Input.GetKeyDown(KeyCode.Mouse0) && KeysActive["Shoot"];
            Keys["Dodge"] = Input.GetKeyDown(KeyCode.LeftShift) && KeysActive["Dodge"];
            Keys["Jump"] = Input.GetKeyDown(KeyCode.Space) && KeysActive["Jump"];

        }

        public Vector3 GetDirection() //gets the direction the player is inputing
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            //float vertical = Input.GetAxisRaw("Vertical");
            return new Vector3(horizontal, 0f, 0f).normalized;
        }


    }
