using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public Transform Target;
    //public GameObject SelectedBox;
    public GameObject Lifebuoy;
    public GameObject Hand;
    public GameObject ShotStart;
    public Main Main;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    


    /// <summary>
    /// 初始化函数
    /// </summary>
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();//GetComponent<NavMeshAgent>(); //获取自身AI组件
        animator = GetComponent<Animator>();     //动画组件
    }


    /// <summary>
    /// 每帧刷新
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButton(0)) //左键
        {
            object ray = Camera.main.ScreenPointToRay(Input.mousePosition); //屏幕坐标转射线
            RaycastHit hit;                                                     //射线投射碰撞
            bool isHit = Physics.Raycast((Ray)ray, out hit);             //射线投射(射线，结构体信息) ；返回bool 值 是否检测到碰撞
            if (isHit)
            {
                Zombie zb = DataManagement.GetInstance().SelectZombie;
                if (hit.collider.gameObject == this.gameObject)
                {
                    if (null != zb)
                    {
                        zb.Selected(false);
                    }
                    Selected(true);
                    if (DataManagement.GetInstance().lifeZZombie == null)
                        DataManagement.GetInstance().lifeZZombie = this;
                    DataManagement.GetInstance().SelectZombie = this;
                }
                //else if (hit.collider.name != this.gameObject.name)
                //{
                //    if (null != zb)
                //    {
                //        DataManagement.GetInstance().SelectZombie.Selected(false);
                //        DataManagement.GetInstance().SelectZombie = null;
                //    }
                //}
            }
        }

        if (null == navMeshAgent || null == Target || IsDead==true)
            return;
        navMeshAgent.SetDestination(Target.position);
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 4.1f &&
            (Lifebuoy == null || (Lifebuoy != null && Lifebuoy.activeSelf == false))) //当前位置 与终点 的  剩余距离<0.5f
        {
            animator.SetBool("walk", false);
            animator.SetBool("atk", true);
            if(Lifebuoy==null || (Lifebuoy!=null && Lifebuoy.activeSelf==false))
            {
                Main.FailGame();
            }
        }
        else
        {
            animator.SetBool("walk", true);
            animator.SetBool("atk", false);
        }
    }

    public RectTransform UIRoot;
    internal void OnDie()
    {
        if (IsDead) return;
        IsDead = true;
        ShotStart.SetActive(false);

        if (Lifebuoy != null) Lifebuoy.gameObject.SetActive(false);
        StartCoroutine(FireAnim());

        Transform hurtPos = transform.Find("HurtPos");
        Vector3 vec3 = Main.UICamera.WorldToScreenPoint(hurtPos.position);
        Vector2 lp = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIRoot, vec3, Main.UICamera, out lp);
        if(transform.name == "Zombie1" || transform.name == "Zombie2")
        {
            Main.ShowHurtTxt(1, lp);
        }
        else
        {
            Main.ShowHurtTxt(2, lp);
            Main.Lead.GetComponent<Lead>().PassLv2 = true;
        }
    }

    public void Selected(bool sele)
    {
        //SelectedBox.SetActive(sele);
    }

    public void SetLifebuoy(bool set)
    {
        Lifebuoy.SetActive(set);
        Hand.SetActive(false);
        Target = null;
        animator.SetBool("walk", false);
        navMeshAgent.isStopped = true;
    }

    public void SetShotStart(bool set)
    {
        ShotStart.SetActive(true);
    }

    public bool IsDead = false;
    IEnumerator FireAnim()
    {
        if(navMeshAgent!=null)
        {
            navMeshAgent.isStopped = true;
        }
        animator.SetBool("die", true);
        for (float timer = 2; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        this.gameObject.SetActive(false);

        if(transform.name == "Zombie3")
        {
            Main.onWinTips();
        }
    }
}
