using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class AStarAlgorithm
    {
        private ANode startNode;
        private ANode destNode;

        private List<ANode> openSet = new List<ANode>();
        private List<ANode> closedSet = new List<ANode>();

        private AMap map;

        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <param name="map"></param>
        public AStarAlgorithm(AMap map)
        {
            this.map = map;
        }

        /// <summary>
        /// 查找开放式集合中H值最小的节点
        /// </summary>
        /// <returns></returns>
        private ANode FindLowest()
        {
            openSet.Sort();
            //Debug.Log(openSet[0].f);
            return openSet[0];

            float minF = openSet[0].f;
            ANode minNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].f < minF)
                {
                    minF = openSet[i].f;
                    minNode = openSet[i];
                }
            }

            return minNode;
        }

        /// <summary>
        /// 将节点的相邻节点添加到开放集合中
        /// </summary>
        /// <param name="node"></param>
        private void AddAdjacent(ANode node)
        {
            for (int i = 0; i < node.adjacent.Count; i++)
            {
                if (closedSet.Contains(node.adjacent[i]))
                    continue;
                else if (openSet.Contains(node.adjacent[i]))
                {
                    int newG = node.adjacent[i].g + (Mathf.Abs(node.Row - node.adjacent[i].Row) + Mathf.Abs(node.Col - node.adjacent[i].Col));
                    if (newG < node.adjacent[i].g)
                    {
                        node.adjacent[i].parent = node;
                        node.adjacent[i].g = newG;
                        node.adjacent[i].f = newG + node.adjacent[i].h;
                    }
                }
                else
                {
                    node.adjacent[i].parent = node;
                    node.adjacent[i].F(startNode, destNode);
                    openSet.Add(node.adjacent[i]);
                }
            }
        }

        /// <summary>
        /// 更新地图
        /// </summary>
        /// <param name="map"></param>
        public void UpdateMap(AMap map)
        {
            this.map = map;
        }

        public void Start(ANode startNode, ANode endNode)
        {
            openSet.Clear();
            closedSet.Clear();

            closedSet.Add(startNode);
            destNode = endNode;
            this.startNode = startNode;

            for (int i = 0; i < map.aNodes.Length; i++)
            {
                map.aNodes[i].Clear();
            }
        }

        public Stack<ANode> Find()
        {
            Stack<ANode> path = new Stack<ANode>();

            ANode currNode = closedSet[0];

            while (currNode != destNode)
            {
                AddAdjacent(currNode);
                if (openSet.Count == 0)
                    break;

                currNode = FindLowest();
                openSet.Remove(currNode);
                closedSet.Add(currNode);
            }

            if (currNode == destNode)
            {
                ANode node = destNode;
                while (node != null)
                {
                    path.Push(node);
                    node = node.parent;
                }
            }
            else
                return null;

            return path;
        }
    }
}
