using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc : MonoBehaviour
{
    public Transform Target;
    //private NavMeshAgent navMeshAgent;
    private Animator animator;
    public GameObject SelectedBox;

    public Zombie RoundZB1;
    public Zombie RoundZB2;

    private bool IsStandUp = false;
    private D3ObjMove MoveControler;

    public Transform HelpUI;


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
    }

    public void PlayMove()
    {
        animator.SetBool("run", true);
    }

    public void PlayIdle()
    {
        animator.SetBool("run", false);
    }


    void Start()
    {
        //navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private float ResetFindPathTime = 1500f;
    private float ResetFindPathTimeVal = 0f;
    public int npcId = 1;
    public RectTransform D3UIRoot;
    public Transform Help3DPosNode;
    public Camera D3UICamera;
    void Update()
    {
        if (HelpUI != null && D3UIRoot != null && HelpUI.gameObject.activeSelf)
        {
            Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3UICamera, Help3DPosNode.transform.position);
            Vector2 lp = new Vector2();
            //vec3.z = 0.0f;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, D3UICamera, out lp);
            HelpUI.transform.localPosition = lp;
        }
        
        if (RoundZB2.IsDead == false) return;

        if(RoundZB1 && RoundZB2 && RoundZB1.IsDead && RoundZB2.IsDead && IsStandUp == false)
        {
            WakeUp();
        }

        float dis = Vector3.Distance(Target.position, transform.position);
        if (dis < 5f)
        {
            MoveControler.StopMove();
            return;
        }

        ResetFindPathTimeVal -= 33.33f;
        if (ResetFindPathTimeVal > 0) return;
        ResetFindPathTimeVal = ResetFindPathTime;
        GridLO lo = MapManager.Ins.ResolveRoleStandGridItem(Target.transform);//朝着目标移动
        Main.Ins.NpcMove(transform, lo, npcId, Target.transform);

        //if (navMeshAgent.enabled && !navMeshAgent.pathPending)
        //{
        //    navMeshAgent.SetDestination(Target.position);
        //    if (navMeshAgent.remainingDistance < 5f)
        //    {
        //        navMeshAgent.isStopped = true;
        //        animator.SetBool("run", false);
        //    }
        //    else
        //    {
        //        navMeshAgent.isStopped = false;
        //        animator.SetBool("run", true);
        //    }
        //}

    }

    public void Selected(bool sele)
    {
        SelectedBox.SetActive(sele);
    }

    public void WakeUp()
    {
        if (null == animator || null == Target)
            return;
        IsStandUp = true;
        StartCoroutine(StandUp());
        if(HelpUI!= null)
            HelpUI.gameObject.SetActive(false);
    }

    IEnumerator StandUp()
    {
        animator.SetBool("standup", true);
        for (float timer = 0.67f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        //navMeshAgent.enabled = true;
        //navMeshAgent.SetDestination(Target.position);
        animator.SetBool("run", true);
    }
}
