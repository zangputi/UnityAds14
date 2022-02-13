using UnityEngine;
using System.Collections;

public class CanvasFollow : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    void Start()
    {
        //设置相对偏移
        offset = target.position - this.transform.position;
    }

    //(-0.3, -32.6, 30.9)
    //9.41f
    //17.0f
    void Update()
    {
        this.transform.position = target.position + offset;
    }
}
