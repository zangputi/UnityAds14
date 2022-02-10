using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManagement
{
    private static DataManagement _instance;

    public static DataManagement GetInstance()
    {
        if (null == _instance)        
            _instance = new DataManagement();
        return _instance;
    }

    private Zombie selectZombie;

    public Zombie SelectZombie { get => selectZombie; set => selectZombie = value; }


    public Zombie lifeZZombie;//救生圈僵尸

    public void SetLifebuoy()
    {
        if (null != lifeZZombie)
        {
            lifeZZombie.SetLifebuoy(true);
        }
    }
    public void SetShotStart()
    {
        if (null != lifeZZombie)
        {
            lifeZZombie.SetShotStart(true);
        }
    }

}
