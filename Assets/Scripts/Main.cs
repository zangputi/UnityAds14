using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game;

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

    public GameObject HurtTxt1;
    public GameObject HurtTxt2;
    public GameObject HurtTxt3;

    public float ScaleP = 2.0f;
    void Start()
    {
        //finger.SetActive(true);
        arrow.SetActive(false);
        //fail.SetActive(false);
       // win.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        ScreenAdapter();
    }

    public Transform H;
    public Transform S;
    public void ScreenAdapter()
    {
        if(Screen.width > Screen.height)
        {
            ScaleP = 1.0f;
            H.gameObject.SetActive(true);
            S.gameObject.SetActive(false);
            Camera.main.orthographicSize = 9.41f;
        }
        else
        {
            ScaleP = 0.5f;
            H.gameObject.SetActive(false);
            S.gameObject.SetActive(true);
            Camera.main.orthographicSize = 17.01f;
        }
    }

    public void onFingerEnd()
    {
        //finger.SetActive(false);
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
        //Lead ld = Lead.GetComponent<Lead>();
        //ld.navMeshAgent.updateRotation = false;
        //ld.navMeshAgent.isStopped = true;
        //ld.animator.SetBool("walk", false);
        //ld.animator.Play("idle", 0, 0.0f);

        IsWin = false;
        GameFinish = true;
        StartCoroutine(Countdown());
    }

    public bool IsWin = false;
    public void WinGame()
    {
        IsWin = true;
        GameFinish = true;
        //win.SetActive(true);
        winTips.SetActive(false);
        ShowResult();
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
        //fail.SetActive(true);
        ShowResult();
    }

    public void ShowResult()
    {
        //if (Screen.width > Screen.height)
        //{
            Transform Success = H.Find("Success");
            Transform Fail = H.Find("Fail");
            Transform UI = H.Find("UI");

            Success.gameObject.SetActive(IsWin);
            Fail.gameObject.SetActive(!IsWin);
            UI.gameObject.SetActive(false);

            Success = S.Find("Success");
            Fail = S.Find("Fail");
            UI = S.Find("UI");

            Success.gameObject.SetActive(IsWin);
            Fail.gameObject.SetActive(!IsWin);
            UI.gameObject.SetActive(false);
        //}
        //else
        //{
        //    H.gameObject.SetActive(false);
        //    S.gameObject.SetActive(true);
        //}
    }

    public void ReGame()
    {
        //SceneManager.LoadScene(0);
    }

    internal void ShotZombie()
    {
        Zombie zb = DataManagement.GetInstance().SelectZombie;
        if(zb != null)
        {
            zb.OnDie();
        }
    }

    internal void NpcUp()
    {
        girl.WakeUp();
        girl1.WakeUp();
    }

    public void ShowHurtTxt(int idx, Vector2 pos)
    {
        GameObject go;
        if(idx == 1)
        {
            go = HurtTxt1;
        }
        else if (idx == 2)
        {
            go = HurtTxt2;
        }
        else
        {
            go = HurtTxt3;
        }

        go.SetActive(true);
        HarmText ht = go.transform.GetComponent<HarmText>(); 
        ht.PlayHarm();
        ht.transform.localPosition = pos;
    }
}
