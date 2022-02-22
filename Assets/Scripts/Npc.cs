using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc : MonoBehaviour
{
    public Transform Target;
    public Transform npc2;
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


    private void Awake()
    {
        MoveControler = transform.GetComponent<D3ObjMove>();
        MoveControler.MoveFunc = new D3ObjMove.ActFunc(PlayMove);
        MoveControler.IdleFunc = new D3ObjMove.ActFunc(PlayIdle);
        if(DirUI)
        {
            DirUI1.gameObject.SetActive(false);
            DirUI.gameObject.SetActive(false);
            NextStep.gameObject.SetActive(false);
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
        animator = GetComponent<Animator>();
        Transform Root = transform.Find("Root");
        if(Root)
        {
            animatorRun = Root.GetComponent<Animator>();
        }
    }

    private float ResetFindPathTime = 1500f;
    private float ResetFindPathTimeVal = 0f;
    public int npcId = 1;
    public RectTransform D3UIRoot;
    public Transform Help3DPosNode;
    public Camera D3UICamera;
    public Camera UICamera;
    public Transform DirUI;
    public Transform DirUI1;
    public Transform NextStep;
    public Transform Boat3D;
    public Vector3 DirPos = new Vector3();
    void Update()
    {
        if(Main.Ins.GameFinish && DirUI)
        {
            DirUI.gameObject.SetActive(false);
            DirUI1.gameObject.SetActive(false);
            NextStep.gameObject.SetActive(false);
        }
        if(DirUI&&DirUI.gameObject.activeSelf)
        {
            Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(UICamera, Boat3D.transform.position);
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
            Debug.Log(ang);
            ang = 180 / Mathf.PI * ang;
            Vector3 localEulerAngles = new Vector3(DirUI.localEulerAngles.x, DirUI.localEulerAngles.y, ang);
            DirUI1.localEulerAngles = localEulerAngles;
            DirUI.localEulerAngles = localEulerAngles;
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

        if(RoundZB1 && RoundZB2 && RoundZB1.IsDead && RoundZB2.IsDead && IsStandUp == false)
        {
            WakeUp();
        }

        float dis = Vector3.Distance(Target.position, transform.position);
        //float dis2 = Vector3.Distance(npc2.position, transform.position);
        if (dis < 5f )
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
        if(npcId == 1)
        {
            Npc1Ready = true;
        }
        else
        {
            Npc2Ready = true;
        }
        IsStandUp = true;
        PlayStandUp();
        //StartCoroutine(StandUp());

        if (HelpUI!= null)
            HelpUI.gameObject.SetActive(false);

        if(DirUI)
        {
            DirUI1.gameObject.SetActive(false);//true
            DirUI.gameObject.SetActive(false);//true
            NextStep.gameObject.SetActive(true);
            PlayLove();
        }
    }

    public Transform Love;
    public Transform LovePos;
    void PlayLove()
    {
        if (Love != null && D3UIRoot != null)
        {
            Vector3 vec3 = RectTransformUtility.WorldToScreenPoint(D3UICamera, LovePos.transform.position);
            Vector2 lp = new Vector2();
            //vec3.z = 0.0f;
            Love.gameObject.SetActive(true);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(D3UIRoot, vec3, D3UICamera, out lp);
            Love.transform.localPosition = lp;
        }
    }

    void StandUp()
    {
        animator.SetBool("standup", true);
        //for (float timer = 0.67f; timer >= 0; timer -= Time.deltaTime)
        //{
        //    yield return 0;
        //}
        //navMeshAgent.enabled = true;
        //navMeshAgent.SetDestination(Target.position);
        //animator.SetBool("run", true);
        PlayRun();
    }

    private void PlayStandUp()
    {
        Npc2ActiveIdleNode();
        StandUp();
        //StartCoroutine(StandUp());
    }

    public void PlayIdle()
    {
        Npc2ActiveIdleNode();
        animator.SetBool("run", false);
    }

    private void PlayRun()
    {
        if (npcId==1)
        {
            Npc2ActiveIdleNode();
            animator.SetBool("run", true);
        }
        else
        {
            Npc2ActiveRunNode();
            if(animatorRun)
                animatorRun.Play("Run", 0, 0.0f);
        }
    }

    private void Npc2ActiveIdleNode()
    {
        animator.enabled = true;
        if (animatorRun)
            animatorRun.enabled = false;
    }

    private void Npc2ActiveRunNode()
    {
        animator.enabled = false;
        if (animatorRun)
            animatorRun.enabled = true;
    }
}
