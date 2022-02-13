using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private D3ObjMove MoveControler;
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
        MoveControler = transform.GetComponent<D3ObjMove>();
        MoveControler.MoveFunc = new D3ObjMove.ActFunc(PlayMove);
        MoveControler.IdleFunc = new D3ObjMove.ActFunc(PlayIdle);
    }

    List<Transform> MoveNodes;
    public void StartMove(List<Transform> mtfs)
    {
        MoveNodes = mtfs;
        MoveControler.MoveTo(mtfs);
        //能到这里说明点的不是僵尸
    }

    Zombie curZb;
    // Update is called once per frame
    void Update()
    {
        if (fire.activeSelf || InTrowLifebuoy)//在开火或者扔泳圈时不可动
            return;
        if (Main.GameFinish || null == MoveControler)
        {
            if (Main.GameFinish && Main.IsWin == true)
            {
                if (Input.GetMouseButton(0))
                {
                    object ray = Camera.main.ScreenPointToRay(Input.mousePosition); //屏幕坐标转射线
                    RaycastHit hit;                                                     //射线投射碰撞
                    bool isHit = Physics.Raycast((Ray)ray, out hit);             //射线投射(射线，结构体信息) ；返回bool 值 是否检测到碰撞
                    if (hit.collider && hit.collider.name == "ferry")
                    {
                        Main.ShowResult();
                    }
                }
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

        if (Lifebuoy.activeSelf == false && clickName == "bucket")//拿枪
        {
            DataManagement.GetInstance().SelectZombie = null;
            float dis = Vector3.Distance(BucketPoint.position, transform.position);
            if(dis < 5f)
            {
                Main.onArrowEnd();
                setFire = false;
            }
        }

        if (Main.GameFinish == false)//救生艇
        {
            float dis = Vector3.Distance(Ferry.position, transform.position);
            if (dis <= 10f)
            {
                //navMeshAgent.isStopped = true;
                //navMeshAgent.updatePosition = false;
                PlayIdle();
                animator.Play("GunIdle", 0, 0.0f);

                Main.WinGame();
                setFire = false;
            }
        }

        //if (null != zb && !navMeshAgent.pathPending)
        if (null != zb)
        {
            float dis = Vector3.Distance(zb.transform.position, transform.position);
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

    internal void OnDie()
    {
        Lifebuoy.SetActive(false);
        setFire = true;
        MoveControler.StopMove(false);
        //animator.SetBool("Gun", true);
        animator.SetBool("die", true);
    }

    private bool TrowLifebuoy = false;
    private bool InTrowLifebuoy = false;
    IEnumerator LifebuoyAnim()
    {
        MoveControler.StopMove(false);

        animator.SetBool("throw", true);
        Lifebuoy.gameObject.SetActive(false);
        LifebuoyAtk.gameObject.SetActive(true);

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
        //TrowLifebuoy = false;
    }

    private bool ShotingFinish = true;
    private bool PassLv1 = false;//打死第一只僵尸后才可通行
    public bool PassLv2 = false;
    IEnumerator FireAnim()
    {
        //animator.SetBool("throw", true);
        animator.Play("GunShot", 0, 0.0f);
        PlayIdle();
        MoveControler.StopMove(false);

        ShotingFinish = false;
        //朝向
        this.transform.LookAt(zb.transform);
        fire.SetActive(true);
        Main.ShotZombie();
        PassLv1 = true;
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
        MoveControler.StopMove();
    }

    private bool IsEquipGun = false;
    public void EquipGun()
    {
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
        animator.SetBool("walk", true);
    }

    public void PlayIdle()
    {
        animator.SetBool("walk", false);
    }
}
