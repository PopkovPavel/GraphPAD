using GraphPAD.GraphData.Model;
using System.Collections.Generic;


namespace GraphPAD.GraphData.Algorithms
{
    public abstract class DFS
    {
        private static void DFSUtil(DataVertex vertex, HashSet<DataVertex> visited, GraphZone graph)
        {
            if (MainPage.isAlgorithmsOn == false) return;
            // if (visited.Find(item => item.Item1 == vertex && item.Item2 == true) != null) return;
            //Dispatcher.Invoke(() => progressBar.Value = 100/(GraphArea.VertexList.Count-test+1));
            if (visited.Contains(vertex)) return; 
            else visited.Add(vertex);
            foreach (var edge in graph.EdgesList.Keys)
            {
                if (edge.Source == vertex
                    && !visited.Contains(edge.Target))
                {
                    MainPage.algorithmEdgesList.Add(edge);
                    MainPage.algorithmResult += $"({vertex.Text}->{edge.Target.Text})";
                    DFSUtil(edge.Target, visited, graph);
                }
            }
        }
        public static void CalculateDFS(DataVertex start, GraphZone graph)
        {
            MainPage.algorithmEdgesList.Clear();
            try
            {
                MainPage.algorithmResult = $"Результат поиска в глубину из вершины \"{start.Text}\":\n";
                HashSet<DataVertex> visitedList = new HashSet<DataVertex>();
                DFSUtil(start, visitedList, graph);

            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

    }
}
