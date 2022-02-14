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
    private void Update()
    {
        this.transform.LookAt(target.transform);
    }

}
