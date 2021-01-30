using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


    public class KeyBindings
    {
        private static System.Random rng = new System.Random();  
        public Dictionary<string, bool> Keys = new Dictionary<string, bool>() {
            {"Shoot", false },
            {"Dodge", false },
            {"Jump", false }
        };

        public Dictionary<string, bool> KeysActive = new Dictionary<string, bool>() {
            {"Shoot", true },
            {"Dodge", true },
            {"Jump", true },
            {"Movement", true }
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
            if(KeysActive["Movement"])
                return new Vector3(horizontal, 0f, 0f).normalized;
            return Vector3.zero;
        }

        public Vector3 GetLookDir()
        {
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;

            Vector3 cameraPositionInWorld =  camera.ScreenToWorldPoint(new Vector3(mouseX, mouseY, cameraDif));
            cameraPositionInWorld = new Vector3(cameraPositionInWorld.x, cameraPositionInWorld.y,0);
            Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y,0);
            Vector3 dir = (cameraPositionInWorld - playerPos).normalized;
            return dir;
        }
        public (float, bool)  GetLookAngle()
        {

            Vector3 dir = GetLookDir();
            float cos = Mathf.Acos(dir.x)*Mathf.Rad2Deg;
            float sin = Mathf.Asin(dir.y)*Mathf.Rad2Deg;
            if(dir.x >0)
                return (sin, false);
            else    
                return (sin,true);

        }

        public void SetActiveKeys(string keyCode, bool newState)
        {
           KeysActive[keyCode] = newState;
        }

        private bool CanRemove(string key)
        {
            if(key == "Movement")
            {
                if(!KeysActive["Dodge"])
                    return false;
  
                
            }
            if(key == "Dodge")
            {
                if(!KeysActive["Movement"])
                    return false;
                
            }
            if(key != "Movement" && KeysActive["Movement"] && CountActiveKeys() == 2)
                return false;
            
            return true;
        }

        public ModuleType RemoveRandomKey()
        {
            ModuleType savedType = ModuleType.EMPTY;
            string savedKey = "";
            ModuleType returnType =  ModuleType.EMPTY;
            List<KeyValuePair<string, bool>> keys = KeysActive.ToList();
            Shuffle(keys);

            foreach(KeyValuePair<string, bool> pair in keys)
            {
                if(pair.Value)
                {
                    if(CanRemove(pair.Key))
                    {
                        KeysActive[pair.Key] = false;
                        returnType = GetType(pair.Key);
                        break;
                    }
                    else
                    {
                        savedType = GetType(pair.Key);
                        savedKey = pair.Key;
                    }
                }
            }
            if(savedType != ModuleType.EMPTY && returnType == ModuleType.EMPTY)
            {
                KeysActive[savedKey] = false;
                return savedType;
            }


            return returnType;
            
        }


        public  void Shuffle<T>(IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }

        private ModuleType GetType(string code)
        {
            ModuleType returnType = ModuleType.EMPTY;
            switch(code)
                        {
                                case "Movement":
                                    returnType = ModuleType.MOVEMENT;
                                    break;
                        
                                case "Dodge":
                                    returnType =  ModuleType.DASH;
                                    break;
                            
                                case "Jump":
                                    returnType =  ModuleType.JUMP;
                                    break;
                            
                                case "Shoot":
                                returnType =  ModuleType.SHOOT;
                                break;
                            
                        }
            return returnType;
        }
        public bool NoActiveKeys()
        {
            List<KeyValuePair<string, bool>> keys = KeysActive.ToList();
            foreach(KeyValuePair<string, bool> pair in keys)
            {
                if(KeysActive[pair.Key])
                    return false;
            }
            return true;
        }

        public int CountActiveKeys()
        {
            int count = 0;
            List<KeyValuePair<string, bool>> keys = KeysActive.ToList();
            foreach(KeyValuePair<string, bool> pair in keys)
            {
                if(KeysActive[pair.Key])
                    count++;
            }
            return count;
        }

    }
