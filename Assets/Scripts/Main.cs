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
    public Zombie zb2;
    public Zombie zb3;
    public Zombie zb4;
    public Zombie zb5;
    public GameObject fail;
    public GameObject win;

    public GameObject HurtTxt1;
    public GameObject HurtTxt2;
    public GameObject HurtTxt3;

    public float ScaleP = 2.0f;
    public Camera UICamera;

    private static Main _ins;
    public static Main Ins { get => _ins; }
    Main()
    {
        _ins = this;
    }
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
    public Transform BgM;
    public Camera D3UICamera;
    public Transform HelpUI;
    public void ScreenAdapter()
    {
        if(Screen.width > Screen.height)
        {
            ScaleP = 1.0f;
            H.gameObject.SetActive(true);
            S.gameObject.SetActive(false);
            //UICamera.orthographicSize = Camera.main.orthographicSize = 9.41f;
            D3UICamera.orthographicSize = Camera.main.orthographicSize = 13f;
        }
        else
        {
            ScaleP = 0.5f;
            H.gameObject.SetActive(false);
            S.gameObject.SetActive(true);
            D3UICamera.orthographicSize = UICamera.orthographicSize = Camera.main.orthographicSize = 28.5f;
            //UICamera.orthographicSize = Camera.main.orthographicSize = 18.06f;
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
        lead1.EquipGun();
        //zb1.Target = lead1.transform;
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
        //winTips.SetActive(false);
        //ShowResult();
        //玩家点击了
    }

    public bool IsNpcDead = false;
    IEnumerator Countdown()
    {        
        for (float timer = 0.1f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        if(IsNpcDead == false)
        {
            lead1.OnDie();
        }
        for (float timer = 0.1f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        //fail.SetActive(true);
        ShowResult();
    }

    public void ShowResult()
    {
        winTips.SetActive(false);
        HelpUI.gameObject.SetActive(false);
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

        BgM.gameObject.SetActive(false);

        zb1.GetComponent<Animator>().speed = 0.0f;
        zb1.transform.Find("IdleSound").gameObject.SetActive(false);
        zb2.GetComponent<Animator>().speed = 0.0f;
        zb2.transform.Find("IdleSound").gameObject.SetActive(false);
        zb3.GetComponent<Animator>().speed = 0.0f;
        zb3.transform.Find("IdleSound").gameObject.SetActive(false);
        zb4.GetComponent<Animator>().speed = 0.0f;
        zb4.transform.Find("IdleSound").gameObject.SetActive(false);
        zb5.GetComponent<Animator>().speed = 0.0f;
        zb5.transform.Find("IdleSound").gameObject.SetActive(false);

        girl.GetComponent<Animator>().speed = 0.0f;
        girl.transform.Find("HelpSound").gameObject.SetActive(false);
        girl1.GetComponent<Animator>().speed = 0.0f;
        girl1.transform.Find("HelpSound").gameObject.SetActive(false);
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

    public void TorusHide()
    {
        lead1.TorusHide();
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

    public void ZBMove(Zombie zb, GridLO endLO)
    {

    }

    public void NpcMove(Npc npc, GridLO endLO)
    {

    }

    public Transform TiSi;
    public void ShowTiShi(Vector3 clickPos)
    {
        if(TiSi)
        {
            ReplayPS(TiSi.Find("1"));
            ReplayPS(TiSi.Find("2"));
            ReplayPS(TiSi.Find("3"));
            ReplayPS(TiSi.Find("4"));
            TiSi.position = new Vector3(clickPos.x, TiSi.position.y, clickPos.z);
        }
    }

    private void ReplayPS(Transform tf)
    {
        ParticleSystem ps = tf.GetComponent<ParticleSystem>();
        ps.Simulate(0.0f);
        ps.Play();
    }

    public Transform MapContent;
    public void LeaderMove(GridLO endLo, bool click=false)
    {
        if (lead1.InTrowLifebuoy) return;
        if (GameFinish || endLo==null || (click && (lead1.Guide1.gameObject.activeSelf == true || lead1.Guide2.gameObject.activeSelf == true))) return;
        //Vector3 leaderPos = MapContent.InverseTransformPoint(Lead.transform.position);
        //float gridSize = MapManager.Ins.GridSize;
        //float minX = MapManager.Ins.MapMinX;
        //float minY = MapManager.Ins.MapMinY;

        //float rolePosX = leaderPos.x - minX;
        //float rolePosY = leaderPos.y - minY;
        //int roleX = Mathf.CeilToInt(rolePosX / gridSize); //角色所在列索引
        //int roleY = Mathf.CeilToInt(rolePosY / gridSize); //角色所在列索引
        //int idx = roleX + roleY * MapManager.Ins.XMax;
        //GameObject obj = MapManager.Ins.Creater.GridItems[idx];
        //GridLO StartLo = obj.transform.GetComponent<GridClicker>().lo;
        GridLO StartLo = MapManager.Ins.ResolveRoleStandGridItem(Lead.transform);
        List<Transform> lineTfs = MapManager.Ins.FindPath8(StartLo, endLo);
        if (lineTfs.Count == 0) return;
        lead1.StartMove(lineTfs);
        //for(int k=0; k< lineTfs.Count; k++)
        //{
        //    Transform tf = lineTfs[k];
        //    tf.GetComponent<MeshRenderer>().material.color = Color.green;
        //}
    }
    public void ZBMove(Transform tf, GridLO endLo)
    {
        GridLO StartLo = MapManager.Ins.ResolveRoleStandGridItem(tf);
        List<Transform> lineTfs = MapManager.Ins.FindPath8(StartLo, endLo);
        if (lineTfs.Count == 0) return;
        Zombie zb = tf.GetComponent<Zombie>();
        zb.StartMove(lineTfs);
    }


    public void NpcMove(Transform tf, GridLO endLo, int npcId, Transform target)
    {
        Vector3 pos = new Vector3();
        if(npcId == 1)
        {
            pos = new Vector3(1.0f, 0.0f, 0.0f);
        }
        else
        {
            pos = new Vector3(-1.0f, 0.0f, 0.0f);
        }
        pos = target.TransformPoint(pos);
        GridLO StartLo = MapManager.Ins.ResolveRoleStandGridItem1(pos);
        List<Transform> lineTfs = MapManager.Ins.FindPath8(StartLo, endLo);
        if (lineTfs.Count == 0) return;
        Npc npc = tf.GetComponent<Npc>();
        npc.StartMove(lineTfs);
    }

}
