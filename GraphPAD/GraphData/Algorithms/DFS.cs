using GraphPAD.GraphData.Model;
using System.Collections.Generic;


namespace GraphPAD.GraphData.Algorithms
{
    /// <summary>
    /// Depth First Search abstract class
    /// </summary>
    public abstract class DFS
    {
        private static void DFSUtility(DataVertex vertex, HashSet<DataVertex> visited, GraphZone graph)
        {
            if (MainPage.isAlgorithmsOn == false) return;
            if (visited.Contains(vertex)) return;
            else visited.Add(vertex);
            foreach (var edge in graph.EdgesList)
            {
                if (edge.Key.Source == vertex
                    && !visited.Contains(edge.Key.Target))
                {
                    MainPage.algorithmEdgesList.Add(edge.Key);//$"\n{minEdge.Source.Text}--({minEdge.Weight})-->{minEdge.Target.Text}"//
                    MainPage.algorithmResult += $"\n{vertex.Text}--({edge.Key.Weight})-->{edge.Key.Target.Text}";
                    DFSUtility(edge.Key.Target, visited, graph);
                }
            }
        }
        /// <summary>
        /// Поиск пути алгоритма "Поиск в глубину"
        /// </summary>
        /// <param name="start">Начальная вершина</param>
        /// <param name="graph">Изначальный граф</param>
        public static void CalculateDFS(DataVertex start, GraphZone graph)
        {
            MainPage.algorithmEdgesList.Clear();
            try
            {
                MainPage.algorithmResult = $"GraphPAD.Properties.Language.ResultDFS(Результат поиска в глубину из точки) \"{start.Text}\":\n";
                HashSet<DataVertex> visitedList = new HashSet<DataVertex>();
                DFSUtility(start, visitedList, graph);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
    }
}
