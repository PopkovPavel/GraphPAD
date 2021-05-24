using GraphPAD.GraphData.Model;

namespace GraphPAD.GraphData.Algorithms
{
    public class DijkstraVertexInfo
    {/// <summary>
     /// Вершина
     /// </summary>
        public DataVertex Vertex { get; set; }

        /// <summary>
        /// Непосещенная вершина
        /// </summary>
        public bool IsUnvisited { get; set; }

        /// <summary>
        /// Сумма весов ребер
        /// </summary>
        public int EdgesWeightSum { get; set; }

        /// <summary>
        /// Предыдущая вершина
        /// </summary>
        public DataVertex PreviousVertex { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="vertex">Вершина</param>
        public DijkstraVertexInfo(DataVertex vertex)
        {
            Vertex = vertex;
            IsUnvisited = true;
            EdgesWeightSum = int.MaxValue;
            PreviousVertex = null;
        }
    }
}
