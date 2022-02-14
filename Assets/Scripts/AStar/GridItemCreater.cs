using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GridItemCreater : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform MinXY;
    public Transform MaxXY;
    public Transform GridItem1;//可通行
    public Transform GridItem2;//可通行
    public Transform GridContent;//所有格子父节点
    public Recoder GridReader;
    void Start()
    {
        //MinXY = transform.Find("MinXY");
        //MaxXY = transform.Find("MaxXY");

        //GridItem1 = transform.Find("GridItem1");
        //GridItem2 = transform.Find("GridItem2");
        //GridContent = transform.Find("GridContent");
        //if (GridLOList==null)
        //{
        //    GridLOList = new List<GridLO>();
        //}
    }

    private Vector2 MinV3 = new Vector2();
    private Vector2 MaxV3 = new Vector2();

    public int TotalX;
    public int TotalY;
    private float GirdItemSizeX;
    private float GirdItemSizeY;
    private void Awake()
    {
        MinV3.x = MinXY.localPosition.x;
        MinV3.y = MinXY.localPosition.y;

        MaxV3.x = MaxXY.localPosition.x;
        MaxV3.y = MaxXY.localPosition.y;

        MinXY.gameObject.SetActive(false);
        MaxXY.gameObject.SetActive(false);
        GridItem1.gameObject.SetActive(false);
        GridItem2.gameObject.SetActive(false);
        
        if(GridReader && GridReader.GridLOList.Count>0)
        {
            for(int k=0; k<GridReader.GridLOList.Count; k++)
            {
                GridLO lo = new GridLO();
                GridLO t = GridReader.GridLOList[k];
                lo.IsWall = t.IsWall;
                lo.X = t.X;
                lo.Y = t.Y;
                recoder.GridLOList.Add(lo);
            }
        }

        GirdItemSizeX = GridItem1.localScale.x;
        GirdItemSizeY = GridItem1.localScale.z;

        GridItems = new List<GameObject>();
        InitGrid();
        if(GridClicker.IsPlaying)
            MapManager.Ins.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Recoder recoder;
    public List<GameObject> GridItems;
    void InitGrid()
    {
        float lenX = MaxV3.x - MinV3.x;
        float lenY = MaxV3.y - MinV3.y;

        TotalX = Mathf.CeilToInt(Mathf.Abs(MaxV3.x - MinV3.x) / GirdItemSizeX);
        TotalY = Mathf.CeilToInt(Mathf.Abs(MaxV3.y - MinV3.y) / GirdItemSizeY);

        for(int i=0; i<TotalY; i++)
        {
            for (int k = 0; k<TotalX; k++)
            {
                GameObject tgo;
                if((k+i)%2==0)
                {
                    tgo = GridItem1.gameObject;
                }
                else
                {
                    tgo = GridItem2.gameObject;
                }
                GameObject obj = Instantiate(tgo, GridContent.transform);
                obj.SetActive(true);
                int idx = (k + 1 + i * TotalX);
                obj.transform.name = idx + "";

                Vector3 pos = new Vector3();
                pos.x = MinV3.x + k * GirdItemSizeX + GirdItemSizeX * 0.5f;
                pos.y = MinV3.y + i * GirdItemSizeY + GirdItemSizeX * 0.5f;

                obj.transform.localPosition = pos;
                GridClicker lo = obj.transform.GetComponent<GridClicker>();
                if(recoder.GridLOList.Count >= idx)
                {
                    lo.lo = recoder.GridLOList[idx - 1];
                    //lo.lo.IsWall = recoder.GridLOList[idx - 1].IsWall;
                    //lo.lo.X = recoder.GridLOList[idx - 1].X;
                    //lo.lo.Y = recoder.GridLOList[idx - 1].Y;
                    lo.Draw();
                    //recoder.GridLOList[idx - 1] = lo.lo;

                    //画
                    //lo.lo = recoder.GridLOList[idx - 1];
                    ////lo.lo.IsWall = recoder.GridLOList[idx - 1].IsWall;
                    ////lo.lo.X = recoder.GridLOList[idx - 1].X;
                    ////lo.lo.Y = recoder.GridLOList[idx - 1].Y;
                    //lo.Draw();
                    ////recoder.GridLOList[idx - 1] = lo.lo;
                }
                else
                {
                    lo.lo.X = k;
                    lo.lo.Y = i;
                    recoder.GridLOList.Add(lo.lo);
                }
                GridItems.Add(obj);
            }
        }
    }
}
