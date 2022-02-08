using UnityEngine;
using System.Collections;

public class CameraFlow : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    void Start()
    {
        //设置相对偏移
        offset = target.position - this.transform.position;
    }

    Vector3 vec3;
    Vector3 HV3 = new Vector3(-4.1f, -32.6f, 30.9f);//-20.8, 32.6, -66.3//R 40.705
    Vector3 SV3 = new Vector3(-0.3f, -32.6f, 30.9f);//-17.68544, 32.68333, -65.96378//R 40.705
    //(-0.3, -32.6, 30.9)
    //9.41f
    //17.0f
    void Update()
    {
        if(Screen.width>Screen.height)
        {
            this.transform.position = target.position - HV3;
        }
        else
        {
            this.transform.position = target.position - SV3;
        }
    }
}
