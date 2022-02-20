using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc2 : MonoBehaviour
{
    public Transform Target;
    public Transform npc1;
    //private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Animator animatorRun;
    public GameObject SelectedBox;

    public Zombie RoundZB1;
    public Zombie RoundZB2;

    private bool IsStandUp = false;
    private D3ObjMove MoveControler;
    public static bool Npc1Ready = false;
    public static bool Npc2Ready = false;

    public Transform HelpUI;
    public Transform ActNormal;
    public Transform ActRun;


    private void Awake()
    {
        MoveControler = transform.GetComponent<D3ObjMove>();
        MoveControler.MoveFunc = new D3ObjMove.ActFunc(PlayMove);
        MoveControler.IdleFunc = new D3ObjMove.ActFunc(PlayIdle);
        if (DirUI)
        {
            DirUI1.gameObject.SetActive(false);
            DirUI.gameObject.SetActive(false);
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
        PlayRun();
    }


    void Start()
    {
        //navMeshAgent = GetComponent<NavMeshAgent>();
        animator = ActNormal.GetComponent<Animator>();
        Transform Root = ActRun.transform.Find("Root");
        if (Root)
        {
            animatorRun = Root.GetComponent<Animator>();
        }

        animator.Play("Trembling", 0, 0.0f);
    }

    private float ResetFindPathTime = 1500f;
    private float ResetFindPathTimeVal = 0f;
    public int npcId = 1;
    public RectTransform D3UIRoot;
    public Transform Help3DPosNode;
    public Camera D3UICamera;
    public Transform DirUI;
    public Transform DirUI1;
    public Transform Boat3D;
    public Vector3 DirPos = new Vector3();
    void Update()
    {
        if (Main.Ins.GameFinish && DirUI)
        {
            DirUI.gameObject.SetActive(false);
            DirUI1.gameObject.SetActive(false);
        }
        if (DirUI && DirUI.gameObject.activeSelf)
        {
            Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3UICamera, Boat3D.transform.position);
            Vector2 lp = new Vector2();
            //vec3.z = 0.0f;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, D3UICamera, out lp);
            //DirUI.transform.localPosition = lp;
            Vector2 v2 = DirUI.transform.localPosition;
            Vector2 dir = lp - v2;
            dir.Normalize();

            float ang = Mathf.Acos(Vector2.Dot(Vector2.down, dir));
            if (lp.x < v2.x)
            {
                ang = -ang;
            }
            ang = 180 / Mathf.PI * ang;
            Vector3 localEulerAngles = new Vector3(DirUI.localEulerAngles.x, DirUI.localEulerAngles.y, ang);
            DirUI1.localEulerAngles = localEulerAngles;

        }

        if (HelpUI != null && D3UIRoot != null && HelpUI.gameObject.activeSelf)
        {
            Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3UICamera, Help3DPosNode.transform.position);
            Vector2 lp = new Vector2();
            //vec3.z = 0.0f;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, D3UICamera, out lp);
            HelpUI.transform.localPosition = lp;
        }

        if (RoundZB2.IsDead == false || RoundZB1.IsDead == false) return;

        if (RoundZB1 && RoundZB2 && RoundZB1.IsDead && RoundZB2.IsDead && IsStandUp == false)
        {
            WakeUp();
        }

        float dis = Vector3.Distance(Target.position, transform.position);
        float dis1 = Vector3.Distance(npc1.position, transform.position);
        if (dis < 5f || dis1 < 1f)
        {
            MoveControler.StopMove();
            return;
        }

        ResetFindPathTimeVal -= 33.33f;
        if (ResetFindPathTimeVal > 0) return;
        ResetFindPathTimeVal = ResetFindPathTime;
        GridLO lo = MapManager.Ins.ResolveRoleStandGridItem(Target.transform);//朝着目标移动
        Main.Ins.NpcMove(transform, lo, npcId, Target.transform);
    }

    public void Selected(bool sele)
    {
        SelectedBox.SetActive(sele);
    }

    public void WakeUp()
    {
        if (null == animator || null == Target)
            return;
        if (npcId == 1)
        {
            Npc1Ready = true;
        }
        else
        {
            Npc.Npc2Ready = true;
        }
        IsStandUp = true;
        PlayStandUp();
        //StartCoroutine(StandUp());

        if (HelpUI != null)
            HelpUI.gameObject.SetActive(false);

        if (DirUI)
        {
            DirUI1.gameObject.SetActive(true);
            DirUI.gameObject.SetActive(true);
        }
    }

    private void PlayStandUp()
    {
        Npc2ActiveIdleNode();
        //Npc.Npc2Ready = true;
        //StartCoroutine(StandUp());
        StandUp();
    }

    void StandUp()
    {
        perActName = "standup";
        //animator.Play("standup", 0, 0.0f);
        //for (float timer = 0.67f; timer >= 0; timer -= Time.deltaTime)
        //{
        //    yield return 0;
        //}
        //navMeshAgent.enabled = true;
        //navMeshAgent.SetDestination(Target.position);
        //animator.SetBool("run", true);
        PlayRun();
    }

    private string perActName = "";
    public void PlayIdle()
    {
        if (perActName == "Idle") return;
        perActName = "Idle";
        Npc2ActiveIdleNode();
        animator.SetBool("run", false);
    }

    private void PlayRun()
    {
        if (perActName == "Run") return;
        perActName = "Run";
        Npc2ActiveRunNode();
        animator.SetBool("run", true);
        if (animatorRun)
            animatorRun.Play("Run", 0, 0.0f);
    }

    private void Npc2ActiveIdleNode()
    {
        ActNormal.gameObject.SetActive(true);
        ActRun.gameObject.SetActive(false);
        //animator.enabled = true;
        //if (animatorRun)
        //    animatorRun.enabled = false;
    }

    private void Npc2ActiveRunNode()
    {
        ActNormal.gameObject.SetActive(false);
        ActRun.gameObject.SetActive(true);
        //animator.enabled = false;
        //if (animatorRun)
        //    animatorRun.enabled = true;
    }

    public void StopAni()
    {
        animator.speed = 0.0f;
        animatorRun.speed = 0.0f;
    }

    public void StopHelp()
    {
        ActNormal.transform.Find("HelpSound").gameObject.SetActive(false);
        ActRun.transform.Find("HelpSound").gameObject.SetActive(false);
    }
}
