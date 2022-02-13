using System.Collections;
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
    }


    private bool Moving = false;
    List<Transform> MoveNodes;
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
        Moving = false;
        if(MoveNodes!=null)
            MoveNodes.Clear();

        if (IdleFunc != null && playAni == true)
            IdleFunc.Invoke();
    }

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

        float y = GetAngle(transform.position, CurTF.position);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
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
