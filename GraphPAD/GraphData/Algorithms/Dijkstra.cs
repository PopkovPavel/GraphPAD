using GraphPAD.GraphData.Model;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace GraphPAD
{
    /// <summary>
    /// Информация о вершине в алгоритме Дейкстры
    /// </summary>
    public class DijkstraVertexInfo
    {   /// <summary>
        /// Вершина
        /// </summary>
        public DataVertex Vertex { get; set; }
        /// <summary>
        /// Непосещенная вершина
        /// </summary>
        public bool IsVisited { get; set; }
        /// <summary>
        /// Сумма весов ребер
        /// </summary>
        public int WeightMark { get; set; }
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
            PreviousVertex = null;
            IsVisited = false;
            WeightMark = int.MaxValue;
            Vertex = vertex;
        }
    }
    public abstract class Dijkstra
    {
        /// <summary>
        /// Поиск непосещенной вершины с минимальным значением суммы
        /// </summary>
        /// <returns>Информация о вершине</returns>      
        private static DijkstraVertexInfo FindUnvisitedVertexWithMinMark(List<DijkstraVertexInfo> infos)
        {
            var minValue = int.MaxValue;
            DijkstraVertexInfo minVertexInfo = null;
            foreach (var i in infos)
            {
                if (!i.IsVisited && i.WeightMark < minValue)
                {
                    minVertexInfo = i;
                    minValue = i.WeightMark;
                }
            }
            return minVertexInfo;
        }
        /// <summary>
        /// Инициализация информации
        /// </summary>
        private static List<DijkstraVertexInfo> InitInfo(GraphZone graph)
        {
            try
            {
                var verticesList = new List<DijkstraVertexInfo>();
                foreach (var v in graph.VertexList.Keys)
                {
                    verticesList.Add(new DijkstraVertexInfo(v));
                }
                return verticesList;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "InitInfo ERROR");
            }
            return null;
        }
        /// <summary>
        /// Получение информации о вершине графа
        /// </summary>
        /// <param name="v">Вершина</param>
        /// <returns>Информация о вершине</returns>
        private static DijkstraVertexInfo GetVertexInfo(DataVertex v, List<DijkstraVertexInfo> infos)
        {
            foreach (var i in infos)
            {
                if (i.Vertex == v)
                {
                    return i;
                }
            }

            return null;
        }
        /// <summary>
        /// Поиск кратчайшего пути по вершинам
        /// </summary>
        /// <param name="startVertex">Стартовая вершина</param>
        /// <param name="finishVertex">Финишная вершина</param>
        /// <returns>Кратчайший путь</returns>
        private static void DijkstraUtility(DataVertex startVertex, DataVertex finishVertex, List<DijkstraVertexInfo> verticesInfo, GraphZone graph)
        {
            try
            {
                var first = GetVertexInfo(startVertex, verticesInfo);
                first.WeightMark = 0;
                while (true)
                {
                    if (MainPage.isAlgorithmsOn == false) return;
                    var current = FindUnvisitedVertexWithMinMark(verticesInfo);
                    if (current == null)
                    {
                        break;
                    }

                    SetMarkToNextVertex(current, verticesInfo, graph);
                }
                MainPage.selectedVertex = null;
                GetDijkstraPath(startVertex, finishVertex, verticesInfo, graph);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FindShortestPath ERROR");
            }
        }
        /// <summary>
        /// Вычисление суммы весов ребер для следующей вершины
        /// </summary>
        /// <param name="info">Информация о текущей вершине</param>
        private static void SetMarkToNextVertex(DijkstraVertexInfo info, List<DijkstraVertexInfo> verticesInfo, GraphZone graph)
        {
            try
            {
                info.IsVisited = true;
                foreach (var e in graph.EdgesList.Keys)
                {
                    if (info.Vertex == e.Source)
                    {
                        var nextInfo = GetVertexInfo(e.Target, verticesInfo);
                        var sum = info.WeightMark + (int)e.Weight;
                        if (sum < nextInfo.WeightMark)
                        {
                            nextInfo.WeightMark = sum;
                            nextInfo.PreviousVertex = info.Vertex;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "SetSumToNextVertex ERROR");
            }
        }
        /// <summary>
        /// Формирование пути
        /// </summary>
        /// <param name="startVertex">Начальная вершина</param>
        /// <param name="endVertex">Конечная вершина</param>
        /// <returns>Путь</returns>
        private static void GetDijkstraPath(DataVertex startVertex, DataVertex endVertex, List<DijkstraVertexInfo> verticesInfo, GraphZone graph)
        {
            var path = endVertex.ToString();
            while (startVertex != endVertex)
            {
                if (GetVertexInfo(endVertex, verticesInfo).PreviousVertex != null)
                {
                    var temp = endVertex;
                    endVertex = GetVertexInfo(endVertex, verticesInfo).PreviousVertex;
                    path = $"{endVertex.Text} -> {path}";
                    foreach (var edge in graph.EdgesList.Keys)
                    {
                        if (edge.Source == endVertex &&
                            edge.Target == temp)
                        {
                            MainPage.algorithmEdgesList.Insert(0, edge);
                        }
                    }
                }
                else
                {
                    path = Properties.Language.PathNotExist;
                }
            }

            MainPage.algorithmResult += path;
        }
        /// <summary>
        /// Алгоритм дейкстры
        /// </summary>
        /// <param name="start">стартовая вершина</param>
        /// <param name="end">конечная вершина</param>
        /// <param name="graph">используемый граф</param>
        public static void CalculateDijkstra(DataVertex start, DataVertex end, GraphZone graph)
        {
            try
            {
                MainPage.algorithmResult = "\n" + Properties.Language.ResultDijkstra + $"\"{start.Text}\"" + Properties.Language.ResultDijkstra2 + $"\"{end.Text}\":\n";
                var verticesInfo = InitInfo(graph);
                DijkstraUtility(start, end, verticesInfo, graph);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "CalculateDijkstra ERROR");
            }
        }
    }
}
