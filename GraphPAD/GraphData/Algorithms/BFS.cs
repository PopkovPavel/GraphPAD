using GraphPAD.GraphData.Model;
using System.Collections.Generic;

namespace GraphPAD.GraphData.Algorithms
{   /// <summary>
    /// BreadthFirstSearch
    /// </summary>
    public abstract class BFS
    {
        private static void BFSUtil(DataVertex vertex, GraphZone graph)
        {
            HashSet<DataVertex> visited = new HashSet<DataVertex>();
            var queue = new Queue<DataVertex>();
            queue.Enqueue(vertex);
            if (MainPage.isAlgorithmsOn == false) return;

            while (queue.Count > 0)
            {
                var vertexTemp = queue.Dequeue();
                if (visited.Contains(vertexTemp)) continue;
                visited.Add(vertexTemp);
                foreach(var edge in graph.EdgesList.Keys)
                {
                    if (edge.Source == vertexTemp 
                        && !visited.Contains(edge.Target))
                    {
                        queue.Enqueue(edge.Target);
                        MainPage.algorithmEdgesList.Add(edge);                       
                        MainPage.algorithmResult += $"({vertex.Text}->{edge.Target.Text})";
                    }
                }

            }
        }
        public static void CalculateBFS(DataVertex start,GraphZone graph) 
        {
            MainPage.algorithmEdgesList.Clear();
            try
            {
                MainPage.algorithmResult = $"Результат поиска в ширину из вершины \"{start.Text}\":\n";
                HashSet<DataVertex> visitedList = new HashSet<DataVertex>();
                BFSUtil(start, graph);
                
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
}
    }
}
