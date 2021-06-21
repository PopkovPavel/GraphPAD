using GraphPAD.GraphData.Model;
using System.Collections.Generic;

namespace GraphPAD.GraphData.Algorithms
{
    /// <summary>
    /// Breadth First Search abstract class
    /// </summary>
    public abstract class BFS
    {
        private static void BFSUtility(DataVertex vertex, GraphZone graph)
        {
            HashSet<DataVertex> visitedVertices = new HashSet<DataVertex>();
            var queue = new Queue<DataVertex>();
            queue.Enqueue(vertex);
            visitedVertices.Add(vertex);
            if (MainPage.isAlgorithmsOn == false) return;
            while (queue.Count > 0)
            {
                var vertexTemp = queue.Dequeue();
                // if (visitedVertices.Contains(vertexTemp)) continue;
                //visitedVertices.Add(vertexTemp);
                foreach (var edge in graph.EdgesList.Keys)
                {
                    if (edge.Source == vertexTemp
                        && !visitedVertices.Contains(edge.Target))
                    {
                        queue.Enqueue(edge.Target);
                        visitedVertices.Add(edge.Target);
                        MainPage.algorithmEdgesList.Add(edge);
                        MainPage.algorithmResult += $"\n{vertex.Text}--({edge.Weight})-->{edge.Target.Text}";
                    }
                }

            }
        }
        /// <summary>
        /// Найти путь алгоритма "Поиск в ширину"
        /// </summary>
        /// <param name="start">Начальная вершина</param>
        /// <param name="graph">Изначальный граф</param>
        public static void CalculateBFS(DataVertex start, GraphZone graph)
        {
            MainPage.algorithmEdgesList.Clear();
            try
            {
                MainPage.algorithmResult = "\n" + Properties.Language.ResultBFS + $"\"{start.Text}\":";
                HashSet<DataVertex> visitedList = new HashSet<DataVertex>();
                BFSUtility(start, graph);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
    }
}
