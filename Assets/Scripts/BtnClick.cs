using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Lead ld;
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickBtn()
    {
        if(ld.Guide1.gameObject.activeSelf)
        {
            DataManagement.GetInstance().SelectZombie = null;
            ld.ToTakeGun = true;

            ld.WalkTo3DGun();
        }
        else if(ld.Guide2.gameObject.activeSelf)
        {
            ld.LockZb1 = true;
        }
    }
}
