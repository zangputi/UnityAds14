using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc : MonoBehaviour
{
    public Transform Target;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public GameObject SelectedBox;

    public Zombie RoundZB1;
    public Zombie RoundZB2;

    private bool IsStandUp = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(RoundZB1 && RoundZB2 && RoundZB1.IsDead && RoundZB2.IsDead && IsStandUp == false)
        {
            WakeUp();
        }

        if (navMeshAgent.enabled && !navMeshAgent.pathPending)
        {
            navMeshAgent.SetDestination(Target.position);
            if (navMeshAgent.remainingDistance < 5f)
            {
                navMeshAgent.isStopped = true;
                animator.SetBool("run", false);
            }
            else
            {
                navMeshAgent.isStopped = false;
                animator.SetBool("run", true);
            }
        }

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
    }

    IEnumerator StandUp()
    {
        animator.SetBool("standup", true);
        for (float timer = 0.67f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(Target.position);
        animator.SetBool("run", true);
    }
}
