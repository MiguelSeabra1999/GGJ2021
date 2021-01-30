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

        private float cameraDif;
        private Camera camera;
        GameObject player;


        public KeyBindings(GameObject player)
        {
            this.player = player;
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
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

        public (float, bool)  GetLookAngle()
        {
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;

            Vector3 cameraPositionInWorld =  camera.ScreenToWorldPoint(new Vector3(mouseX, mouseY, cameraDif));
            cameraPositionInWorld = new Vector3(cameraPositionInWorld.x, cameraPositionInWorld.y,0);
            Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y,0);
            Vector3 dir = (cameraPositionInWorld - playerPos).normalized;

            float cos = Mathf.Acos(dir.x)*Mathf.Rad2Deg;
            float sin = Mathf.Asin(dir.y)*Mathf.Rad2Deg;
            if(dir.x >0)
                return (sin, false);
            else    
                return (sin,true);

        }



    }
