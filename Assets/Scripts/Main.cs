using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera SceneCamera;
    public RectTransform UIParent;
    public GameObject Lead;
    public RectTransform leadUI;

    public GameObject finger;
    public GameObject arrow;
    public GameObject gun;
    public GameObject winTips;

    public Lead lead1;
    public Npc girl;
    public Npc girl1;

    public Zombie zb1;
    public GameObject fail;
    public GameObject win;
    void Start()
    {
        finger.SetActive(true);
        arrow.SetActive(false);
        fail.SetActive(false);
        win.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onFingerEnd()
    {
        finger.SetActive(false);
        arrow.SetActive(true);
    }

    public void onArrowEnd()
    {
        arrow.SetActive(false);
        gun.SetActive(false);
        zb1.Target = lead1.transform;
    }

    public void onWinTips()
    {
        winTips.SetActive(true);        
    }

    public bool GameFinish = false;
    public void FailGame()
    {
        GameFinish = true;
        StartCoroutine(Countdown());
    }
    public void WinGame()
    {
        GameFinish = true;
        win.SetActive(true);
        winTips.SetActive(false);
    }

    IEnumerator Countdown()
    {        
        for (float timer = 0.5f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        lead1.OnDie();
        for (float timer = 2f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        fail.SetActive(true);
    }

    public void ReGame()
    {
        //SceneManager.LoadScene(0);
    }

    internal void ShotZombie()
    {
        zb1.OnDie();
    }

    internal void NpcUp()
    {
        girl.WakeUp();
        girl1.WakeUp();
    }
}
