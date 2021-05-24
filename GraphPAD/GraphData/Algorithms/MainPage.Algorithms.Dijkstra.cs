using GraphPAD.GraphData.Algorithms;
using GraphPAD.GraphData.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphPAD
{
    public partial class MainPage : Window
    {
        public static List<DijkstraVertexInfo> infos;
        /// <summary>
        /// Поиск непосещенной вершины с минимальным значением суммы
        /// </summary>
        /// <returns>Информация о вершине</returns>
        public DijkstraVertexInfo FindUnvisitedVertexWithMinSum()
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
        void InitInfo()
        {
            infos = new List<DijkstraVertexInfo>();
            foreach (var v in GraphArea.VertexList.Keys)
            {
                infos.Add(new DijkstraVertexInfo(v));
            }
        }
        /// <summary>
        /// Получение информации о вершине графа
        /// </summary>
        /// <param name="v">Вершина</param>
        /// <returns>Информация о вершине</returns>
        DijkstraVertexInfo GetVertexInfo(DataVertex v)
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
        public string FindShortestPath(DataVertex startVertex, DataVertex finishVertex)
        {
            InitInfo();
            var first = GetVertexInfo(startVertex);
            first.EdgesWeightSum = 0;
            while (true)
            {
                var current = FindUnvisitedVertexWithMinSum();
                if (current == null)
                {
                    break;
                }

                SetSumToNextVertex(current);
            }
            MessageBox.Show(GetPath(startVertex, finishVertex));
            selectedVertex = null;
            return GetPath(startVertex, finishVertex);
        }

        /// <summary>
        /// Вычисление суммы весов ребер для следующей вершины
        /// </summary>
        /// <param name="info">Информация о текущей вершине</param>
        void SetSumToNextVertex(DijkstraVertexInfo info)
        {
            info.IsUnvisited = false;           
            foreach (var e in GraphArea.EdgesList.Keys)
            {
                if(info.Vertex == e.Source)
                {
                    var nextInfo = GetVertexInfo(e.Target);
                    var sum = info.EdgesWeightSum + (int)e.Weight;
                    if (sum < nextInfo.EdgesWeightSum)
                    {
                        nextInfo.EdgesWeightSum = sum;
                        nextInfo.PreviousVertex = info.Vertex;
                    }
                }              
            }
        }

        /// <summary>
        /// Формирование пути
        /// </summary>
        /// <param name="startVertex">Начальная вершина</param>
        /// <param name="endVertex">Конечная вершина</param>
        /// <returns>Путь</returns>
        string GetPath(DataVertex startVertex, DataVertex endVertex)
        {
            var path = endVertex.ToString();
            while (startVertex != endVertex)
            {
                if(GetVertexInfo(endVertex).PreviousVertex != null)
                {
                    endVertex = GetVertexInfo(endVertex).PreviousVertex;
                    path = endVertex.Text + "->" + path;
                }
                else
                {
                    path = "Такого пути не существует";
                }
            }

            algorithmResult += path;
            return path;
        }
        public void CalculateDijkstra(DataVertex end)
        {
            test = 0;
            algorithmEdgesList.Clear();
            try
            {
                algorithmResult = $"Результат поиска кратчайшего пути из вершины \"{selectedVertex.Text}\" до {end.Text}:";

                InitInfo();
                Thread thread = new Thread(x => FindShortestPath(selectedVertex, end));
                thread.Start();
            }
            catch
            {

            }
            //    GraphArea.
        }

    }
}
