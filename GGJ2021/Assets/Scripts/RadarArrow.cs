using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarArrow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    public float MAXDIST;
    public float MINDIST;
    private float Offset = -90;
    
    Camera camera;
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("secondCam").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null){Destroy(gameObject);return;}
        float mag = (target.transform.position - transform.position).magnitude ;
        Vector3 pos = camera.WorldToViewportPoint(target.transform.position);
        if( mag < MINDIST || mag> MAXDIST)
        {
            if(target!=null)
                transform.parent.SendMessage("RemoveTracking", target);
            Destroy(gameObject);
        }

        else
        {
            transform.rotation = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z);
            LookAt(target.transform.position);
        }
    }

    private void LookAt(Vector3 targetPos)
    {
        Vector3 diff = (targetPos - transform.position).normalized;
        if(diff.x >= 0  && diff.y >= 0)
            transform.rotation =  Quaternion.Euler(0,0,Mathf.Acos(diff.x)*Mathf.Rad2Deg+Offset);
        else if(diff.x <= 0  && diff.y >= 0)
        {
            //bugged
            transform.rotation =  Quaternion.Euler(0,180,(-1*Mathf.PI-Mathf.Acos(diff.x))*Mathf.Rad2Deg+Offset);
        }else if(diff.x <= 0  && diff.y<= 0)
        {
            transform.rotation =  Quaternion.Euler(0,180,(-1*Mathf.PI+Mathf.Acos(diff.x))*Mathf.Rad2Deg+Offset);
        }else
             transform.rotation =  Quaternion.Euler(0,0,(-1*Mathf.Acos(diff.x))*Mathf.Rad2Deg+Offset);

    }
}
