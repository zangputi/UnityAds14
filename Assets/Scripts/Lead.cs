using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Lead : MonoBehaviour
{

    public Main Main;
    public GameObject Lifebuoy;
    public GameObject fire;


    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Zombie zb;

    private LeadState lState = LeadState.Melee;
    private bool setFire = true;
    private string clickName = "";

    public enum LeadState
    {
        Melee,
        shot,
    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();     //动画组件
        setFire = true;
    }

    Zombie curZb;
    // Update is called once per frame
    void Update()
    {
        if (fire.activeSelf)
            return;
        if (Main.GameFinish || null == navMeshAgent)
            return;
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
            if(hit.collider)
            {
                clickName = hit.collider.name;
                zb = DataManagement.GetInstance().SelectZombie;
                navMeshAgent.isStopped = false;
                curZb = hit.collider.transform.GetComponent<Zombie>();
                
                if (curZb != null && null != zb)
                {
                    navMeshAgent.SetDestination(zb.transform.position);
                }
                else
                {
                    if (PassLv1 == false && (hit.point.z >= -17f || hit.point.x> -13.3f)) return;//第一个僵尸没打死不可前往流程2   
                    navMeshAgent.SetDestination(hit.point);
                    DataManagement.GetInstance().SelectZombie = null;
                }
                animator.SetBool("walk", true);
            }
        }

        if (Lifebuoy.activeSelf==false && clickName == "bucket" && navMeshAgent.remainingDistance < 5f)//拿枪
        {
            Main.onArrowEnd();
            setFire = false;
        }
        
        if (clickName == "ferry" && navMeshAgent.remainingDistance < 0.5f)//救生艇
        {
            Main.WinGame();
            setFire = false;
        }

        //if (null != zb && !navMeshAgent.pathPending)
        if (null != zb && navMeshAgent.remainingDistance>= 0.2f)
        {
            switch (lState)
            {
                case LeadState.Melee:
                    if (navMeshAgent.remainingDistance < 10f && TrowLifebuoy == false)
                    {
                        TrowLifebuoy = true;
                        StartCoroutine(LifebuoyAnim());
                    }
                    break;
                case LeadState.shot:
                    if (zb == null || zb.IsDead || !ShotingFinish) break;
                    if (navMeshAgent.remainingDistance < 12f)
                    {
                        navMeshAgent.isStopped = true;
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
            if (navMeshAgent.remainingDistance < 0.2f)
            {
                animator.SetBool("walk", false);
            }
        }
    }

    internal void OnDie()
    {
        Lifebuoy.SetActive(false);
        setFire = true;
        navMeshAgent.isStopped = true;
        animator.SetBool("die", true);
    }

    private bool TrowLifebuoy = false;
    IEnumerator LifebuoyAnim()
    {
        animator.SetBool("throw", true);
        navMeshAgent.isStopped = true;

        this.transform.LookAt(zb.transform);
        navMeshAgent.updateRotation = false;
        for (float timer = 1f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        navMeshAgent.updateRotation = true;
        Lifebuoy.SetActive(false);
        lState = LeadState.shot;
        DataManagement.GetInstance().SetLifebuoy();
        Main.onFingerEnd();
        //Main.NpcUp();

        animator.SetBool("walk", false);
        navMeshAgent.isStopped = true;
    }

    private bool ShotingFinish = true;
    private bool PassLv1 = false;//打死第一只僵尸后才可通行
    IEnumerator FireAnim()
    {
        //animator.SetBool("throw", true);
        animator.Play("Throw", 0, 0.0f);
        animator.SetBool("walk", false);
        navMeshAgent.updateRotation = false;
        navMeshAgent.isStopped = true;

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
        navMeshAgent.updateRotation = true;

        ShotingFinish = true;
    }
}
