using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 节点类
    /// </summary>
    public class ANode : System.IComparable
    {
        /// <summary>
        /// 行
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public ANode parent = null;
        /// <summary>
        /// 相邻节点
        /// </summary>
        public List<ANode> adjacent = new List<ANode>();
        /// <summary>
        /// 曼哈顿距离
        /// </summary>
        public int h = 0;

        public int g = 0;

        public int f = 0;

        public void F(ANode startNode,ANode endNode)
        {
            h = Mathf.Abs(endNode.Row - Row) + Mathf.Abs(endNode.Col - Col);
            g = Mathf.Abs(startNode.Row - Row) + Mathf.Abs(startNode.Col - Col);

            f = g + h;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            parent = null;
            h = 0;
            g = 0;
            f = 0;
            //adjacent.Clear();
        }

        public int CompareTo(object obj)
        {
            ANode node = obj as ANode;

            if (f - node.f < 0)
                return -1;
            else if (f - node.f == 0)
                return 0;
            else
                return 1;
        }
    }
}
