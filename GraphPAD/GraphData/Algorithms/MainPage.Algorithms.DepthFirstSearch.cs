using GraphPAD.GraphData.Algorithms;
using GraphPAD.GraphData.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphPAD
{
    public partial class MainPage : Window
    {
        

        private void DFSUtil(DataVertex vertex, List<MyTuple> visited)
        {
           // if (visited.Find(item => item.Item1 == vertex && item.Item2 == true) != null) return;
            if (flagNegr == true) return;
            

            if (visited.Find(item => item.Vertex == vertex && item.Visited == false) == null) return; else visited.Find(item => item.Vertex == vertex && item.Visited == false).Visited = true;
            foreach(var vertex1 in GraphArea.EdgesList.Keys) 
            {
                var temp = visited.Find(item => item.Vertex == vertex1.Target 
                    && item.Visited == true);
                if (vertex1.Source == vertex 
                    && temp == null)
                {
                    algorithmEdgesList.Add(vertex1);
                    algorithmResult += $"({vertex.Text}->{vertex1.Target.Text})";
                    DFSUtil(vertex1.Target, visited);
                }
            }         
        }
        public void CalculateDFS(DataVertex start)
        {

            try
            {
                algorithmResult = $"Результат поиска в глубину из вершины \"{start.Text}\":";
                List<MyTuple> visitedList = new List<MyTuple>();
                int i = 0;
                foreach (var vertex in GraphArea.VertexList)
                {
                    visitedList.Add(new MyTuple(vertex.Key, false));
                    i++;
                }

                DFSUtil(start, visitedList);
                chatCount += 1;
                ChatsScrollView.ScrollToBottom();
                ChatBox.AppendText($"Вы: {algorithmResult}\n\n");
                SocketConnector.SendMessage(algorithmResult);
            }
            catch
            {

            }
            //    GraphArea.
        }

    }
}
