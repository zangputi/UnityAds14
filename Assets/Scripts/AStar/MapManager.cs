using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    MapManager()
    {
        _ins = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private static MapManager _ins;
    public static MapManager Ins { get => _ins; }

    AStar.Point[,] mapPoints;
    public int XMax;//列个数
    public int YMax;//行个数
    public float GridSize = 0.1f;
    public float MapMinX = 9999999f;//左下角的点坐标
    public float MapMinY = 9999999f;//左下角的点坐标

    public Recoder recorder;
    public GridItemCreater Creater;
    private void Awake()
    {
        this.XMax = 67;
        this.YMax = 37;
    }

    public void Init()
    {
        mapPoints = new AStar.Point[XMax, YMax];
        for (int k = 0; k < recorder.GridLOList.Count; k++)
        {
            var grid = recorder.GridLOList[k];
            int x = k % XMax;
            int y = Mathf.FloorToInt(k / XMax);
            GameObject obj = Creater.GridItems[k];
            Vector3 vec3 = obj.transform.localPosition;
            if (MapMinX > vec3.x)
            {
                MapMinX = vec3.x;
            }
            if (MapMinY > vec3.y)
            {
                MapMinY = vec3.y;
            }

            var mapPoint = new AStar.Point(grid.X, grid.Y, obj);
            mapPoint.IsWall = grid.IsWall;
            mapPoints[mapPoint.X, mapPoint.Y] = mapPoint;
        }
    }

    public List<Transform> FindPath(GridLO startGrid, GridLO endGrid)
    {
        return FindPath8(startGrid, endGrid);
    }

    public List<Transform> FindPath8(GridLO startGrid, GridLO endGrid)
    {
        List<Transform> ret = new List<Transform>();

        var startpoint = mapPoints[startGrid.X, startGrid.Y];
        var endPoint = mapPoints[endGrid.X, endGrid.Y];
        bool b = AStar.AStarWrapper.Ins.FindPath(startpoint, endPoint, mapPoints, XMax, YMax);

        if (b)
        {
            var addPoint = endPoint;
            int startIdx = startGrid.X + startGrid.Y * XMax;
            do
            {
                var index = GetIndex(addPoint.Y, addPoint.X, mapPoints);
                if (index == startIdx)
                {
                    break;
                }
                else
                {
                    addPoint = addPoint.Parent;
                }
                ret.Insert(0, Creater.GridItems[index].transform);
            } while (true);
        }
        return ret;
    }
    public int GetIndex(int row, int col, int[,] tempMap)
    {
        int cols = tempMap.GetLength(1);
        return row * cols + col;
    }

    public int GetIndex(int row, int col, AStar.Point[,] tempMap)
    {
        //int cols = map.GetLength(1);
        //return (MapData.Ins.maxY  - 1 - row) * cols + col;

        int cols = tempMap.GetLength(0);
        return row * cols + col;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public GridLO ResolveRoleStandGridItem(Transform tf)
    {
        GridLO lo;
        Vector3 pos = Main.Ins.MapContent.InverseTransformPoint(tf.position);
        float gridSize = MapManager.Ins.GridSize;
        float minX = MapManager.Ins.MapMinX;
        float minY = MapManager.Ins.MapMinY;

        float rolePosX = pos.x - minX;
        float rolePosY = pos.y - minY;
        int roleX = Mathf.CeilToInt(rolePosX / gridSize); //角色所在列索引
        int roleY = Mathf.CeilToInt(rolePosY / gridSize); //角色所在列索引
        int idx = roleX + roleY * MapManager.Ins.XMax;
        GameObject obj = MapManager.Ins.Creater.GridItems[idx];
        GridLO StartLo = obj.transform.GetComponent<GridClicker>().lo;
        return StartLo;
    }

    public GridLO ResolveRoleStandGridItem1(Vector3 pos)
    {
        GridLO lo;
        pos = Main.Ins.MapContent.InverseTransformPoint(pos);
        float gridSize = MapManager.Ins.GridSize;
        float minX = MapManager.Ins.MapMinX;
        float minY = MapManager.Ins.MapMinY;

        float rolePosX = pos.x - minX;
        float rolePosY = pos.y - minY;
        int roleX = Mathf.CeilToInt(rolePosX / gridSize); //角色所在列索引
        int roleY = Mathf.CeilToInt(rolePosY / gridSize); //角色所在列索引
        int idx = roleX + roleY * MapManager.Ins.XMax;
        GameObject obj = MapManager.Ins.Creater.GridItems[idx];
        GridLO StartLo = obj.transform.GetComponent<GridClicker>().lo;
        return StartLo;
    }
}
