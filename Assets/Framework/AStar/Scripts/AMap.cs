

namespace Common
{
    /// <summary>
    /// 图
    /// </summary>
    public class AMap
    {
        /// <summary>
        /// 列数
        /// </summary>
        public int cols = 0;
        /// <summary>
        /// 行数
        /// </summary>
        public int rows = 0;
        /// <summary>
        /// 行优先节点数组
        /// </summary>
        public ANode[] aNodes;

        public AMap(int[,] mapArray)
        {
            rows = mapArray.GetLength(0);
            cols = mapArray.GetLength(1);

            aNodes = new ANode[mapArray.Length];

            //分配节点的行号和列号
            for (int i = 0; i < aNodes.Length; i++)
            {
                ANode node = new ANode();
                node.Row = i / cols;
                node.Col = i - node.Row * cols;

                aNodes[i] = node;
            }

            int row, col;
            //分配邻居节点
            for (int j = 0; j < aNodes.Length; j++)
            {
                row = aNodes[j].Row;
                col = aNodes[j].Col;

                //1表示有障碍物，不能通过
                //0表示无障碍物，可以通过
                if (mapArray[row, col] != 1)
                {
                    //相邻上方的一个节点
                    if (row > 0 && mapArray[row - 1, col] != 1)
                        aNodes[j].adjacent.Add(aNodes[(row - 1) * cols + col]);
                    //相邻右边的一个节点
                    if (col + 1 < cols && mapArray[row, col + 1] != 1)
                        aNodes[j].adjacent.Add(aNodes[row * cols + col + 1]);
                    //相邻下方的一个节点
                    if (row + 1 < rows && mapArray[row + 1, col] != 1)
                        aNodes[j].adjacent.Add(aNodes[(row + 1) * cols + col]);
                    //相邻左边的一个节点
                    if (col > 0 && mapArray[row, col - 1] != 1)
                        aNodes[j].adjacent.Add(aNodes[row * cols + col - 1]);
                }
            }
        }


    }
}