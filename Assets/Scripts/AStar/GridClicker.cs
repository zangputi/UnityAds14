using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[Serializable]
//public struct GridLO
//{
//    public bool IsWall;
//    public int X;
//    public int Y;
//}
public class GridClicker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        lo = new GridLO();
        Material mtr = transform.GetComponent<MeshRenderer>().materials[0];
        if(mtr.color==null)
        {
            return;
        }
        DefaultCol = mtr.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GridLO lo;
    public Color DefaultCol;
    private bool MouseD;

    private static bool GridMapMouseD;
    public static bool  CurMouseEventIsWall;
    public static GridClicker StartClickGrid;
    public static float ClickDtT = 300.0f;
    public static float ClickDt = 0;
    public void OnMouseDown()
    {
        GridClicker.StartClickGrid = this;
        GridClicker.CurMouseEventIsWall = !lo.IsWall;
        if (IsPlaying && ClickDt <= 0f)
        {
            ClickDt = ClickDtT;
            DataManagement.GetInstance().SelectZombie = null;
            Main.Ins.LeaderMove(lo, true);
            return;
        }

        if (MouseD==true || GridMapMouseD == true)
        {
            return;
        }
        GridMapMouseD = true;
        MouseD = true;
        //GridClicker.StartClickGrid = this;
        //GridClicker.CurMouseEventIsWall = !lo.IsWall;
        lo.IsWall = GridClicker.CurMouseEventIsWall;
        Draw();
    }
    private void OnMouseUp()
    {
        GridClicker.StartClickGrid.MouseD = false;
        GridClicker.GridMapMouseD = false;
        GridClicker.StartClickGrid = null;
    }
    private void OnMouseExit()
    {
    }

    public static bool IsPlaying = true;
    private void OnMouseOver()
    {
        if(!GridClicker.GridMapMouseD || IsPlaying)
        {
            return;
        }
        lo.IsWall = GridClicker.CurMouseEventIsWall;
        Draw();
    }

    public void Draw()
    {
        if (IsPlaying == false)
            transform.GetComponent<MeshRenderer>().materials[0].color = lo.IsWall ? Color.red : DefaultCol;
    }
}
