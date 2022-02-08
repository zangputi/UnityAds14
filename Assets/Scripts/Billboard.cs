using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        cameraToLookAt = Camera.main;
    }
    public Camera cameraToLookAt;
    void Update()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
    }

}
