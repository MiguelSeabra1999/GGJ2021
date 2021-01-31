using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  enum ModuleType {MOVEMENT, SHOOT, DASH, JUMP, EMPTY};

public class Module : MonoBehaviour
{

    public float timeToLive = 3;

    public float startBlinkPercentage = 0.7f;
    public float startBlinkSpeed = 0.1f;
    public float endBlinkSpeed = 0.05f;
    public float turnBackOnTime = 0.1f;
    private Animator animator;

    [HideInInspector] public ModuleType type = ModuleType.EMPTY;
    private float birthTime;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        birthTime = Time.time;
        sprite = GetComponent<SpriteRenderer>();
        GameObject P = GameObject.FindGameObjectWithTag("Player");
        if(P){
            P.BroadcastMessage("AddMe", gameObject);
        }
       
        this.StartCoroutine(LifeCycle(gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetModuleType(ModuleType type)
    {
        animator = this.GetComponent<Animator>();
        this.type = type;
        switch(type)
        {
            case ModuleType.DASH:
                animator.SetTrigger("Dash");
            break;
            case ModuleType.JUMP:
             animator.SetTrigger("Jump");
            break;
            case ModuleType.SHOOT:
            animator.SetTrigger("Shoot");
            break;
        }
    }

    IEnumerator LifeCycle(GameObject obj)
    {
        float blinkSpeed = startBlinkSpeed;
        float timePercent;
        yield return new WaitForSeconds(timeToLive * startBlinkPercentage);
        
        while(Time.time < birthTime + timeToLive)
        {
            timePercent =(Time.time - birthTime)  / timeToLive;
            blinkSpeed =  Mathf.Lerp(startBlinkSpeed, endBlinkSpeed, timePercent);
            yield return new WaitForSeconds(blinkSpeed);
            sprite.enabled = false;
            yield return new WaitForSeconds(turnBackOnTime);
            sprite.enabled = true;
        }
        Destroy(obj);
    }


}
