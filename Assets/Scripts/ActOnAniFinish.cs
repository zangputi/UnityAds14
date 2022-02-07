using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActOnAniFinish : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AniEndDestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }
    public void AniEndDestroySelf()
    {
        Destroy(transform.gameObject);
    }

    public void AniEndUnactiveParent()
    {
        transform.parent.gameObject.SetActive(false);
    }
    public void AniEndUnactiveSelf()
    {
        transform.gameObject.SetActive(false);
    }
}
