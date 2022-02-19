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
    public GameObject ShotStartUI;
    public Transform VertigoUI;
    public Main Main;

    //private NavMeshAgent navMeshAgent;
    private D3ObjMove MoveControler;
    private Animator animator;
    public Camera D3Camera;

    private void Awake()
    {
        MoveControler = transform.GetComponent<D3ObjMove>();
        MoveControler.MoveFunc = new D3ObjMove.ActFunc(PlayMove);
        MoveControler.IdleFunc = new D3ObjMove.ActFunc(PlayIdle);

        animator = GetComponent<Animator>();     //动画组件
        if (TA)
        {
            GridLO lo = MapManager.Ins.ResolveRoleStandGridItem(TB.transform);//朝着目标移动
            Main.ZBMove(transform, lo);
        }
    }

    List<Transform> MoveNodes;
    public void StartMove(List<Transform> mtfs)
    {
        MoveNodes = mtfs;
        MoveControler.MoveTo(mtfs);
    }

    public void PlayMove()
    {
        animator.SetBool("walk", true);
    }

    public void PlayIdle()
    {
        animator.SetBool("walk", false);
    }

    /// <summary>
    /// 初始化函数
    /// </summary>
    void Start()
    {
        //navMeshAgent = GetComponent<NavMeshAgent>();//GetComponent<NavMeshAgent>(); //获取自身AI组件
    }

    public float ResetFindPathTime = 1500f;
    private float ResetFindPathTimeVal = 0f;
    /// <summary>
    /// 每帧刷新
    /// </summary>
    void Update()
    {
        if (Main.Ins.GameFinish) return;

        if (ShotStartUI != null)
        {
            Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3Camera, ShotStart.transform.position);
            //vec3.x -= Screen.width * 0.5f;
            //vec3.y -= Screen.height * 0.5f;
            Vector2 lp = new Vector2();
            //vec3.z = 0.0f;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, D3Camera, out lp);
            ShotStartUI.transform.localPosition = lp;
        }

        if (VertigoUI != null)
        {
            Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3Camera, ShotStart.transform.position);
            //vec3.x -= Screen.width * 0.5f;
            //vec3.y -= Screen.height * 0.5f;
            Vector2 lp = new Vector2();
            //vec3.z = 0.0f;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, D3Camera, out lp);
            VertigoUI.transform.localPosition = lp;
        }
        

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

        if (null == MoveControler || null == Target || IsDead == true)
        {
            MoveToA();
            MoveToB();
            return;
        }
        ResetFindPathTimeVal -= 33.33f;
        if (ResetFindPathTimeVal > 0) return;
        ResetFindPathTimeVal = ResetFindPathTime;
        GridLO lo = MapManager.Ins.ResolveRoleStandGridItem(Target.transform);//朝着目标移动
        Main.ZBMove(transform, lo);
        //navMeshAgent.SetDestination(Target.position);
        float dis = Vector3.Distance(Target.transform.position, transform.position);
        if (dis < 4.1f &&
            (Lifebuoy == null || (Lifebuoy != null && Lifebuoy.activeSelf == false))) //当前位置 与终点 的  剩余距离<0.5f
        {
            PlayIdle();
            animator.SetBool("atk", true);
            Target = null;
            if (Lifebuoy == null || (Lifebuoy != null && Lifebuoy.activeSelf == false))
            {
                Main.FailGame();
            }
            MoveControler.StopMove();
        }
        else
        {
            PlayMove();
            animator.SetBool("atk", false);
        }
    }

    public Transform TA;
    public Transform TB;
    private void MoveToA()
    {
        if (!TA) return;
        float dis = Vector3.Distance(TA.transform.position, transform.position);
        if(Lifebuoy != null && Lifebuoy.activeSelf == false)
        {
            if(dis < 5f)
            {
                GridLO lo = MapManager.Ins.ResolveRoleStandGridItem(TB.transform);//朝着目标移动
                Main.ZBMove(transform, lo);
            }
        }
    }
    private void MoveToB()
    {
        if (!TB) return;
        float dis = Vector3.Distance(TB.transform.position, transform.position);
        if (Lifebuoy != null && Lifebuoy.activeSelf == false)
        {
            if (dis < 5f)
            {
                GridLO lo = MapManager.Ins.ResolveRoleStandGridItem(TA.transform);//朝着目标移动
                Main.ZBMove(transform, lo);
            }
        }
    }

    public RectTransform UIRoot;
    public RectTransform D3UIRoot;
    internal void OnDie()
    {
        if (IsDead) return;
        IsDead = true;
        ShotStart.SetActive(false);
        if(ShotStartUI)
        {
            ShotStartUI.SetActive(false);
            VertigoUI.gameObject.SetActive(false);
        }

        if (Lifebuoy != null) Lifebuoy.gameObject.SetActive(false);
        StartCoroutine(FireAnim());

        //Transform hurtPos = transform.Find("HurtPos");
        //Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(Main.UICamera, Main.Lead.transform.position);
        //vec3.x -= Screen.width * 0.5f;
        ////vec3.y -= Screen.height * 0.5f;
        //Vector2 lp = new Vector2();
        //vec3.z = 0.0f;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(UIRoot, vec3, Main.UICamera, out lp);
        //if(transform.name == "Zombie1" || transform.name == "Zombie2")
        //{
        //    Main.ShowHurtTxt(1, lp);
        //}
        //else
        //{
        //    Main.ShowHurtTxt(2, lp);
        //    Main.Lead.GetComponent<Lead>().PassLv2 = true;
        //}


        Transform hurtPos = transform.Find("HurtPos");
        Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        //vec3.x -= Screen.width * 0.5f;
        //vec3.y -= Screen.height * 0.5f;
        Vector2 lp = new Vector2();
        vec3.z = 0.0f;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, Camera.main, out lp);
        if (transform.name == "Zombie1" || transform.name == "Zombie2")
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
        PlayIdle();
        MoveControler.StopMove();
        VertigoUI.gameObject.SetActive(true);
    }

    public void SetShotStart(bool set)
    {
        ShotStart.SetActive(true);
        if(ShotStartUI)
        {
            ShotStartUI.SetActive(true);
            VertigoUI.gameObject.SetActive(false);
        }
    }

    public bool IsDead = false;
    IEnumerator FireAnim()
    {
        if (MoveControler != null)
        {
            MoveControler.StopMove();
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
