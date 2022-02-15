using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }
    public Transform target;
    // Update is called once per frame
    private void LateUpdate()
    {
        //this.transform.LookAt(target.transform);
        var lookPos = Camera.main.transform.position - transform.position;
        lookPos.y = 0; 
        transform.rotation = Quaternion.LookRotation(lookPos, Camera.main.transform.up) ;
    }

}
