﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Lead : MonoBehaviour
{

    public Main Main;
    public GameObject Lifebuoy;//没套僵尸前的救生圈
    public GameObject LifebuoyAtk;//套僵尸动作的救生圈
    public GameObject fire;


    //public NavMeshAgent navMeshAgent;
    public Animator animator;
    private Zombie zb;
    public Transform Ferry;

    private LeadState lState = LeadState.Melee;
    private bool setFire = true;
    private string clickName = "";

    public Transform Gun;
    public Transform BucketPoint;
    public Transform BoatPoint;
    private D3ObjMove MoveControler;

    public Transform Npc1;
    public Transform Npc2;
    public Zombie Z1;
    public Zombie Z2;
    public Zombie Z3;
    public Zombie Z4;
    public Zombie Z5;
    public Transform LvUpEffect;
    public enum LeadState
    {
        Melee,
        shot,
    }

    void Start()
    {
        //navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();     //动画组件
        setFire = true;
    }

    private void Awake()
    {
        GuideLv = 0;
        Guide1.gameObject.SetActive(false);
        Guide2.gameObject.SetActive(false);
        MoveControler = transform.GetComponent<D3ObjMove>();
        MoveControler.MoveFunc = new D3ObjMove.ActFunc(PlayMove);
        MoveControler.IdleFunc = new D3ObjMove.ActFunc(PlayIdle);
    }

    List<Transform> MoveNodes;
    public void StartMove(List<Transform> mtfs)
    {
        MoveNodes = mtfs;
        MoveControler.MoveTo(mtfs);
        if(mtfs.Count > 0)
            Main.Ins.ShowTiShi(mtfs[mtfs.Count-1].position);
        //能到这里说明点的不是僵尸
    }

    public Transform GuideH;
    public Transform Guide1;
    public Transform Guide2;
    public int GuideLv = 0;
    public Camera D3Camera;
    public Transform SceneGun1;
    public Transform SceneGun2;
    public RectTransform D3UIRoot;

    public Transform ShoStart;
    public Transform GuideClicker;
    public bool ToTakeGun = false;
    public bool TakedGun = false;
    public bool LockZb1 = false;

    Zombie curZb;
    public Transform OilBarrelPos;
    public Transform MedicalBoxPos;
    public Transform OilBarrel;
    public Transform MedicalBox;

    public Transform HpRoot;
    public Transform Hp1;
    public Transform Hp2;
    // Update is called once per frame
    void Update()
    {
        //DoFlyItem();
        SynHpBar();

        GridClicker.ClickDt -= 33.33f;
        if (fire.activeSelf || InTrowLifebuoy)//在开火或者扔泳圈时不可动
            return;

        if(GuideLv == 1)
        {
            Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3Camera, SceneGun1.position);
            Vector2 lp = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Guide1.transform as RectTransform, vec3, D3Camera, out lp);

            float GuideW = 50000.0f;
            float x1 = lp.x / GuideW * -1.0f - 0.0025f;
            float y1 = lp.y / GuideW * -1.0f;
            Material mt = Guide1.GetComponent<Image>().material;
            mt.SetFloat("_PosX_1", x1);
            mt.SetFloat("_PosY_1", y1);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot.transform as RectTransform, vec3, D3Camera, out lp);
            GuideClicker.localPosition = lp;
        }
        else if(GuideLv == 2)
        {
            Vector3 cInGuide = Guide2.transform.InverseTransformPoint(ShoStart.transform.position);
            //Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3Camera, ShoStart.position);
            //Vector2 lp = new Vector2();
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(Guide2.transform as RectTransform, vec3, D3Camera, out lp);

            float GuideW = 50000.0f;
            //Vector3 cInGuide = lp;//Guide2.transform.InverseTransformPoint(lp);
            float x1 = cInGuide.x / GuideW * -1.0f;
            float y1 = cInGuide.y / GuideW * -1.0f;
            Material mt = Guide2.GetComponent<Image>().material;
            mt.SetFloat("_PosX_1", x1);
            mt.SetFloat("_PosY_1", y1);

            cInGuide = D3UIRoot.transform.InverseTransformPoint(ShoStart.transform.position);
            GuideClicker.localPosition = cInGuide;
        }

        if (Main.GameFinish || null == MoveControler)
        {
            if (Main.GameFinish && Main.IsWin == true)
            {
                //if (Input.GetMouseButton(0))
                //{
                //    object ray = Camera.main.ScreenPointToRay(Input.mousePosition); //屏幕坐标转射线
                //    RaycastHit hit;                                                     //射线投射碰撞
                //    bool isHit = Physics.Raycast((Ray)ray, out hit);             //射线投射(射线，结构体信息) ；返回bool 值 是否检测到碰撞
                //    if (hit.collider && hit.collider.name == "ferry")
                //    {
                //        Main.ShowResult();
                //    }
                //}
            }
            return;
        }
        curZb = null;
        if (Input.GetMouseButton(0))
        {
            object ray = Camera.main.ScreenPointToRay(Input.mousePosition); //屏幕坐标转射线
            RaycastHit hit;                                                     //射线投射碰撞
            bool isHit = Physics.Raycast((Ray)ray, out hit);             //射线投射(射线，结构体信息) ；返回bool 值 是否检测到碰撞
            //if (isHit)
            //{
            //    //navMeshAgent.SetDestination(hit.point); //AI组件，设置目的地/终点
            //}
            if (hit.collider)
            {
                clickName = hit.collider.name;
                zb = DataManagement.GetInstance().SelectZombie;
                //navMeshAgent.isStopped = false;
                curZb = hit.collider.transform.GetComponent<Zombie>();

                if (curZb != null && null != zb)
                {
                    GridLO EndLo = MapManager.Ins.ResolveRoleStandGridItem(zb.transform);
                    Main.LeaderMove(EndLo);
                }
                else if(clickName == "bucket")
                {
                    GridLO EndLo = MapManager.Ins.ResolveRoleStandGridItem(BucketPoint);
                    Main.LeaderMove(EndLo);
                    clickName = "";
                }
                else if (clickName == "OilBarrel")
                {
                    GridLO EndLo = MapManager.Ins.ResolveRoleStandGridItem(OilBarrelPos);
                    Main.LeaderMove(EndLo);
                    clickName = "";
                }
                else if (clickName == "MedicalBox")
                {
                    GridLO EndLo = MapManager.Ins.ResolveRoleStandGridItem(MedicalBoxPos);
                    Main.LeaderMove(EndLo);
                    clickName = "";
                }
                else 
                {
                    //if (PassLv1 == false && (hit.point.z >= -17f || hit.point.x > -13.3f)) return;//第一个僵尸没打死不可前往流程2  
                    //if (PassLv2 == false && (hit.point.x > -0.7f)) return;//第一个僵尸没打死不可前往流程2   
                    //navMeshAgent.SetDestination(hit.point);
                    //DataManagement.GetInstance().SelectZombie = null;
                }
            }
        }

        float dis = 0.0f;
        //if (Lifebuoy.activeSelf == false && clickName == "bucket")//拿枪
        if (Lifebuoy.activeSelf == false && ToTakeGun==true &&TakedGun==false)//拿枪
        {
            //DataManagement.GetInstance().SelectZombie = null;
            dis = Vector3.Distance(BucketPoint.position, transform.position);
            if(dis < 1f)
            {
                ToTakeGun = false;
                TakedGun = true;
                Main.onArrowEnd();
                PlayLvUpEff();
                setFire = false;
            }
        }
        //医疗箱 
        dis = Vector3.Distance(MedicalBoxPos.position, transform.position);
        if (MedicalBox.gameObject.activeSelf && dis < 2f)
        {
            MedicalBox.gameObject.SetActive(false);
            Hp2.gameObject.SetActive(true);
            PlayLvUpEff();
            StartFlyItem(1);
        }
        //油箱
        dis = Vector3.Distance(OilBarrelPos.position, transform.position);
        if (OilBarrel.gameObject.activeSelf && dis < 2f)
        {
            OilBarrel.gameObject.SetActive(false);
            PlayLvUpEff();
            StartFlyItem(2);
        }

        if (Main.GameFinish == false)//救生艇
        {
            dis = Vector3.Distance(BoatPoint.position, transform.position);
            if (dis <= 5f && Lifebuoy.activeSelf == false)
                //if (dis <= 5f && Lifebuoy.activeSelf == false && Npc.Npc1Ready && Npc.Npc2Ready)
                {
                //navMeshAgent.isStopped = true;
                //navMeshAgent.updatePosition = false;
                PlayIdle();
                animator.Play("GunIdle", 0, 0.0f);
                MoveControler.StopMove();

                Main.WinGame();
                HpRoot.gameObject.SetActive(false);
                Main.ShowResult();
                setFire = false;
            }
            else if (clickName == "ferry" && Lifebuoy.activeSelf == false)
            {
                clickName = "";
                GridLO EndLo = MapManager.Ins.ResolveRoleStandGridItem(BoatPoint);
                Main.LeaderMove(EndLo);
            }
        }

        if(LockZb1)
        {
            LockZb1 = false;
            zb = Z1;
        }
        //if (null != zb && !navMeshAgent.pathPending)
        if (null != zb)
        {
            dis = Vector3.Distance(zb.transform.position, transform.position);
            switch (lState)
            {
                case LeadState.Melee:
                    if (dis < 6f && TrowLifebuoy == false)
                    {
                        TrowLifebuoy = true;
                        InTrowLifebuoy = true;
                        StartCoroutine(LifebuoyAnim());
                    }
                    break;
                case LeadState.shot:
                    if (zb == null || zb.IsDead || !ShotingFinish || !DataManagement.GetInstance().SelectZombie) break;//套泳圈离枪太近bug 
                    if (dis < 12f)
                    {
                        if (!setFire)
                        {
                            setFire = true;
                            StartCoroutine(FireAnim());
                        }
                    }
                    break;
            }
        }
        else
        {
            if (Main.GameFinish)
            {
                PlayIdle();
            }
        }
    }

    public void WalkTo3DGun()
    {
        GridLO EndLo = MapManager.Ins.ResolveRoleStandGridItem(BucketPoint);
        Main.LeaderMove(EndLo);
    }

    internal void OnDie()
    {
        HpRoot.gameObject.SetActive(false);
        Lifebuoy.SetActive(false);
        setFire = true;
        MoveControler.StopMove(false);
        //animator.SetBool("Gun", true);
        //animator.SetBool("die", true);
        if (TakedGun)
        {
            animator.Play("GunDead", 0, 0.0f);
        }
        else
        {
            animator.Play("die", 0, 0.0f);
        }

    }

    private bool TrowLifebuoy = false;
    public bool InTrowLifebuoy = false;
    IEnumerator LifebuoyAnim()
    {
        MoveControler.StopMove(false);

        animator.SetBool("throw", true);
        Lifebuoy.gameObject.SetActive(false);
        LifebuoyAtk.gameObject.SetActive(true);
        GuideClicker.gameObject.SetActive(true);

        this.transform.LookAt(zb.transform);
        for (float timer = 1f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        Lifebuoy.SetActive(false);
        lState = LeadState.shot;
        //DataManagement.GetInstance().SetLifebuoy();
        Main.onFingerEnd();
        //Main.NpcUp();

        PlayIdle();
        InTrowLifebuoy = false;
        animator.SetBool("throw", false);
        MoveControler.StopMove();
        GuideLv = 1;
        Guide1.gameObject.SetActive(true);
        SceneGun2.gameObject.SetActive(false);
        SceneGun1.gameObject.SetActive(true);
        //TrowLifebuoy = false;
    }

    private bool ShotingFinish = true;
    private bool PassLv1 = false;//打死第一只僵尸后才可通行
    public bool PassLv2 = false;
    IEnumerator FireAnim()
    {
        Debug.Log("FireAnim");
        //animator.SetBool("throw", true);
        animator.Play("GunShot", 0, 0.0f);
        PlayIdle();
        MoveControler.StopMove(false);

        ShotingFinish = false;
        //朝向
        this.transform.LookAt(zb.transform);
        fire.SetActive(true);
        Main.ShotZombie();

        float y = MoveControler.GetAngle(transform.position, DataManagement.GetInstance().SelectZombie.transform.position);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);

        PassLv1 = true;
        GuideLv = 3;
        GuideClicker.gameObject.SetActive(false);
        Guide1.gameObject.SetActive(false);
        Guide2.gameObject.SetActive(false);
        //animator.Play("Throw", 0, 0.0f);
        for (float timer = 1; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        //fire.SetActive(false);
        //for (float timer = 0.5f; timer >= 0; timer -= Time.deltaTime)
        //{
        //    yield return 0;
        //}
        setFire = false;

        ShotingFinish = true;

        Z2.Target = Npc1;
        Z4.Target = transform;
        Z5.Target = transform;

        for (float timer = 0.3f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        Z3.Target = Npc2;
    }

    private bool IsEquipGun = false;
    public void EquipGun()
    {
        GuideLv = 2;
        Guide1.gameObject.SetActive(false);
        Guide2.gameObject.SetActive(true);

        Gun.gameObject.SetActive(true);
        IsEquipGun = true;

        animator.SetBool("Gun", true);
        DataManagement.GetInstance().SetShotStart();
    }

    public void TorusHide()
    {
        LifebuoyAtk.gameObject.SetActive(false);
        DataManagement.GetInstance().SetLifebuoy();
    }

    public void PlayMove()
    {
        Debug.Log("PlayMove");
        animator.SetBool("walk", true);
    }

    public void PlayIdle()
    {
        Debug.Log("PlayIdle");
        animator.SetBool("walk", false);
    }

    public Transform LvUpPos;
    public void PlayLvUpEff()
    {
        LvUpEffect.gameObject.SetActive(true);

        Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3Camera, LvUpPos.transform.position);
        Vector2 lp = new Vector2();
        //vec3.z = 0.0f;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, D3Camera, out lp);
        lp.y += 50f;
        LvUpEffect.transform.localPosition = lp;
    }

    public Transform HpPos;
    public void SynHpBar()
    {
        if (HpRoot != null && D3UIRoot != null && HpRoot.gameObject.activeSelf)
        {
            Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3Camera, HpPos.transform.position);
            Vector2 lp = new Vector2();
            //vec3.z = 0.0f;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, D3Camera, out lp);
            HpRoot.transform.localPosition = lp;
        }
    }

    public Image UIOil;
    public Image UIMac;
    public Transform BagPosH;
    public Transform BagPosS;
    public void ToFlyItem(int type )
    {
        //医疗箱
        Transform tar;
        Transform tar1;
        if (type==1)
        {
            tar = MedicalBox;
            tar1 = UIMac.transform;
        }
        else
        {
            tar = OilBarrel;
            tar1 = UIOil.transform;
        }
        tar1.gameObject.SetActive(true);
        Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3Camera, tar.transform.position);
        Vector2 lp = new Vector2();
        //vec3.z = 0.0f;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, D3Camera, out lp);
        UIMac.transform.localPosition = lp;
        UIOil.transform.localPosition = lp;
        //StartFlyItem(tar1);
    }

    public Vector3 UIScreenToWorldPoint(Vector3 uiPostion)
    {
        uiPostion = UICamera.WorldToScreenPoint(uiPostion);
        uiPostion.z = UICamera.transform.localPosition.z;
        uiPostion = Camera.main.ScreenToWorldPoint(uiPostion);
        return uiPostion;
    }

    public Transform FlyItem;
    public Camera UICamera;
    private void StartFlyItem(int type)
    {
        FlyItem.Find("Oil").gameObject.SetActive(false);
        FlyItem.Find("Mec").gameObject.SetActive(false);
        FlyItem.gameObject.gameObject.SetActive(true);
        FlyItem.Find(type == 2 ? "Oil" : "Mec").gameObject.SetActive(true);
        string AniName = "";
        Animator atr = FlyItem.GetComponent<Animator>();
        if (Screen.width > Screen.height)
        {
            AniName = "FlyItemH";
        }
        else
        {
            AniName = "FlyItemS";
        }
        atr.Play(AniName, 0, 0.0f);

        //FlyItem = img;
        //Transform BagPos;
        //if(Screen.width > Screen.height)
        //{
        //    BagPos = BagPosH;
        //}
        //else
        //{
        //    BagPos = BagPosS;
        //}
        //Vector3 BagWps = UICamera.ScreenToWorldPoint(BagPos.position);
        //Dir = BagWps - FlyItem.position;
        //Dir.Normalize();
        //StartFlyPos = FlyItem.position;
        //FlyDis = Vector2.Distance(StartFlyPos, FlyItem.position);
        //FlyDurT = 0.0f;
    }

    public Transform DownloadH;
    public Transform DownloadS;

    //private float FlyDurTT = 1000f;
    //private float FlyDurT = 0f;
    //private Vector3 Dir;
    //private Vector3 StartFlyPos;
    //private float FlyDis;
    //private void DoFlyItem()
    //{
    //    if (!FlyItem)
    //        return;
    //    float dt = 33.33f;
    //    FlyDurT += dt;
    //    float per = FlyDurT / FlyDurTT;

    //    bool end = false;
    //    if(per > 1.0f)
    //    {
    //        per = 1.0f;
    //        end = true;
    //    }
    //    Vector3 curPos = StartFlyPos + per * FlyDis * Dir;
    //    FlyItem.position = curPos;

    //    if (end)
    //    {
    //        FlyItem.gameObject.SetActive(false);
    //        FlyItem = null;
    //    }
    //}
}
