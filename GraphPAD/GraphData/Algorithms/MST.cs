using GraphPAD.GraphData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPAD.GraphData.Algorithms
{
    /// <summary>
    /// Minimum weight spanning tree (Минимальное остовное дерево)
    /// </summary>
    public abstract class MST
    {
        private static void MSTUtil(DataVertex vertex, GraphZone graph)
        {
            HashSet<DataEdge> edgesList = new HashSet<DataEdge>();
            HashSet<DataVertex> verticesList = new HashSet<DataVertex>();
            foreach (var item in graph.EdgesList.Keys)
            {
                edgesList.Add(item);
            }
            foreach (var item in graph.VertexList.Keys)
            {
                verticesList.Add(item);
            }

            HashSet<DataVertex> visitedVertices = new HashSet<DataVertex>();
            HashSet<DataEdge> result = new HashSet<DataEdge>();
            visitedVertices.Add(vertex);
            verticesList.Remove(vertex);
            while (verticesList.Count > 0)
            {
                if (MainPage.isAlgorithmsOn == false) return;
                List<DataEdge> connectedEdges = new List<DataEdge>();
                foreach(var e in edgesList)
                {
                    if (visitedVertices.Contains(e.Source) 
                        && !result.Contains(e) 
                        && !visitedVertices.Contains(e.Target))
                    {
                        connectedEdges.Add(e);
                    }
                }
                if (connectedEdges.Count == 0) break;
                DataEdge minEdge = connectedEdges?[0]; //чтобы не вызвать исключение
                foreach (DataEdge e in connectedEdges) //поиск ребра с меньшей длиной
                {
                    if (e.Weight < minEdge.Weight) minEdge = e;
                }
                result.Add(minEdge); //добавление его в финальный граф

                var nonSelectedVertex = minEdge.Target;

                visitedVertices.Add(nonSelectedVertex);
                verticesList.Remove(nonSelectedVertex);
                edgesList.Remove(minEdge);
                 //добавление этой точки в финальный граф
                MainPage.algorithmResult += $"\n{minEdge.Source.Text}--({minEdge.Weight})-->{minEdge.Target.Text}";
                MainPage.algorithmEdgesList.Add(minEdge);
                //minimalTree = selectedEdges;
            }

        }
         
        public static void CalculateMST(DataVertex start, GraphZone graph)
        {
            MainPage.algorithmEdgesList.Clear();
            try
            {
                MainPage.algorithmResult = $"Результат поиска в глубину из вершины \"{start.Text}\":\n";
                HashSet<DataVertex> visitedList = new HashSet<DataVertex>();
                MSTUtil(start, graph);

            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
    }
}
