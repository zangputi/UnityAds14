﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D3ObjMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Step = Speed * dt;
    }

    public delegate void ActFunc();
    public ActFunc MoveFunc;
    public ActFunc IdleFunc;

    public float Speed = 0.006f;
    //public float StopDis = 0.1f;
    Transform CurTF;
    float dt = 33.33f;
    float Step = 0f;
    private Vector3 TestV3 = new Vector3();
    private Vector3 RealPos = new Vector3();
    // Update is called once per frame
    void Update()
    {
        if(Moving)
        {
            if(CurTF==null)
            {
                StopMove(true);
                return;
            }
            float dis = Vector3.Distance(CurTF.position, transform.position);
            if (dis <= Step)
            {
                RealPos.x = CurTF.position.x;
                RealPos.y = CurTF.position.y;
                RealPos.z = CurTF.position.z;
                transform.position = RealPos;
                if (MoveNodes.Count == 0)
                {
                    StopMove(true);
                    return;
                }
                NextNode();
                Update();
            }
            else
            {
                float moveDis = Speed * dt;
                TestV3.x = transform.position.x;
                TestV3.y = transform.position.y;
                TestV3.z = transform.position.z;
                TestV3 += moveDis* Dir;
                float dis2 = Vector3.Distance(TestV3, CurTF.position);
                if( dis2 < dis)
                {
                    RealPos.x = TestV3.x;
                    RealPos.y = TestV3.y;
                    RealPos.z = TestV3.z;
                    transform.position = RealPos;
                }
                else
                {
                    if (MoveNodes.Count == 0)
                    {
                        StopMove(true);
                        return;
                    }
                    NextNode();
                }
            }
        }

        if (InRoting)
        {
            RotDT -= dt;
            if (RotDT <= 0)
            {
                InRoting = false;
                EV3.x = transform.eulerAngles.x;
                EV3.y = SourceE + ChangeE;
                EV3.z = transform.eulerAngles.z;
                transform.eulerAngles = EV3;
                return;
            }
            float per = 1f - RotDT / RotT;
            EV3.x = transform.eulerAngles.x;
            EV3.y = SourceE + per * ChangeE;
            EV3.z = transform.eulerAngles.z;
            transform.eulerAngles = EV3;
        }
    }
    private Vector3 EV3 = new Vector3();

    private bool Moving = false;
    public List<Transform> MoveNodes;
    public void MoveTo(List<Transform> val)
    {
        RealPos.x = transform.position.x;
        RealPos.y = transform.position.y;
        RealPos.z = transform.position.z;

        MoveNodes = val;
        Moving = true;
        NextNode();

        if (MoveFunc != null)
            MoveFunc.Invoke();
    }

    public void StopMove(bool playAni=true)
    {
        CurTF = null;
        InRoting = false;
        Moving = false;
        if(MoveNodes!=null)
            MoveNodes.Clear();

        if (IdleFunc != null && playAni == true)
            IdleFunc.Invoke();
    }

    private float RotT = 200f;
    private float RotDT = 0.0f;
    private bool InRoting = false;
    private float SourceE = 0f;
    private float ChangeE = 0f; 

    private Vector3 Dir;
    private void NextNode()
    {
        if (MoveNodes.Count == 0)
            return;
        RealPos.x = transform.position.x;
        RealPos.y = transform.position.y;
        RealPos.z = transform.position.z;

        CurTF = MoveNodes[0];
        Dir = CurTF.position - transform.position;
        Dir.Normalize();
        MoveNodes.RemoveAt(0);

        RotDT = RotT;
        InRoting = true;
        SourceE = transform.eulerAngles.y;
        //float y = GetAngle(transform.position, CurTF.position);

        Vector2 dir = new Vector2(CurTF.position.x - transform.position.x, CurTF.position.z - transform.position.z);
        dir.Normalize();
        float ang = Mathf.Acos(Vector2.Dot(Vector2.up, dir));
        if (CurTF.position.x < transform.position.x)
        {
            ang = -ang;
        }
        ang = 180 / Mathf.PI * ang;
        ChangeE = ang - SourceE;
        ChangeE = ChangeE % 360f;
        if (Mathf.Abs(ChangeE) > 180.0f)
        {
            if( ChangeE < 0)
            {
                ChangeE = 360f + ChangeE;
            }
            else
            {
                ChangeE = 360f - ChangeE;
            }
        }
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, ang, transform.eulerAngles.z);

    }


    public float GetAngle(Vector3 a, Vector3 b)
    {
        b.x -= a.x;
        b.z -= a.z;

        float deltaAngle = 0;
        if (b.x == 0 && b.z == 0)
        {
            return 0;
        }
        else if (b.x > 0 && b.z > 0)
        {
            deltaAngle = 0;
        }
        else if (b.x > 0 && b.z == 0)
        {
            return 90;
        }
        else if (b.x > 0 && b.z < 0)
        {
            deltaAngle = 180;
        }
        else if (b.x == 0 && b.z < 0)
        {
            return 180;
        }
        else if (b.x < 0 && b.z < 0)
        {
            deltaAngle = -180;
        }
        else if (b.x < 0 && b.z == 0)
        {
            return -90;
        }
        else if (b.x < 0 && b.z > 0)
        {
            deltaAngle = 0;
        }

        float angle = Mathf.Atan(b.x / b.z) * Mathf.Rad2Deg + deltaAngle;
        return angle;
    }
}
