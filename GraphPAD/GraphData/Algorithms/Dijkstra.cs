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
    public abstract class Dijkstra
    {

        /// <summary>
        /// Поиск непосещенной вершины с минимальным значением суммы
        /// </summary>
        /// <returns>Информация о вершине</returns>      
        private static DijkstraVertexInfo FindUnvisitedVertexWithMinSum(List<DijkstraVertexInfo> infos)
        {
            var minValue = int.MaxValue;
            DijkstraVertexInfo minVertexInfo = null;
            foreach (var i in infos)
            {
                if (i.IsUnvisited && i.EdgesWeightSum < minValue)
                {
                    minVertexInfo = i;
                    minValue = i.EdgesWeightSum;
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
            catch
            {
                MessageBox.Show("Произошла ошибка, возможно граф был изменен");
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
        private static void FindShortestPath(DataVertex startVertex, DataVertex finishVertex, List<DijkstraVertexInfo> verticesInfo, GraphZone graph)
        {
            try
            {
                var first = GetVertexInfo(startVertex, verticesInfo);
                first.EdgesWeightSum = 0;
                while (true)
                {
                    if (MainPage.isAlgorithmsOn == false) return;
                    var current = FindUnvisitedVertexWithMinSum(verticesInfo);
                    if (current == null)
                    {
                        break;
                    }

                    SetSumToNextVertex(current, verticesInfo, graph);
                }
                MainPage.selectedVertex = null;
                GetDijkstraPath(startVertex, finishVertex, verticesInfo, graph);
            }
            catch
            {
                MessageBox.Show("Произошла ошибка, возможно граф был изменен", "FindShortestPath ERROR");
            }
        }

        /// <summary>
        /// Вычисление суммы весов ребер для следующей вершины
        /// </summary>
        /// <param name="info">Информация о текущей вершине</param>
        private static void SetSumToNextVertex(DijkstraVertexInfo info, List<DijkstraVertexInfo> verticesInfo, GraphZone graph)
        {
            try
            {
                info.IsUnvisited = false;
                foreach (var e in graph.EdgesList.Keys)
                {
                    if (info.Vertex == e.Source)
                    {
                        var nextInfo = GetVertexInfo(e.Target, verticesInfo);
                        var sum = info.EdgesWeightSum + (int)e.Weight;
                        if (sum < nextInfo.EdgesWeightSum)
                        {
                            nextInfo.EdgesWeightSum = sum;
                            nextInfo.PreviousVertex = info.Vertex;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Произошла ошибка, возможно граф был изменен", "SetSumToNextVertex ERROR");
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
                    path = "Такого пути не существует";
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
                MainPage.algorithmResult = $"Результат поиска кратчайшего пути из вершины " +
                    $"\"{start.Text}\" в вершину \"{end.Text}\":\n";
                var verticesInfo = InitInfo(graph);
                FindShortestPath(start, end, verticesInfo,graph);
               
            }
            catch
            {
                MessageBox.Show("Произошла ошибка, возможно граф был изменен", "CalculateDijkstra ERROR");
            }
        }
    }
}
