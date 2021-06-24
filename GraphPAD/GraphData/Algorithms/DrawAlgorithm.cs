using GraphPAD.GraphData.Algorithms;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphPAD
{
    public partial class MainPage : Window
    {
        private void StartAlgorithm(GraphX.Controls.VertexControl vc, GraphData.Model.GraphZone graph)
        {
            FixLabelsAndArrows();
            if (isAlgorithmsOn)
            {
                System.Threading.Thread thread = null;
                algorithmEdgesList.Clear();
                var vertex = (GraphData.Model.DataVertex)vc.Vertex;
                switch (AlgorithmHelper.ChoosedAlgorithm)
                {                 
                    case "DFS":
                        {
                            thread = new System.Threading.Thread(x => DFS.CalculateDFS(vertex, graph));            
                            break;
                        }
                    case "BFS":
                        {
                            thread = new System.Threading.Thread(x => BFS.CalculateBFS(vertex, graph));
                            break;
                        }
                    case "MST":
                        {
                            thread = new System.Threading.Thread(x => MST.CalculateMST(vertex, graph));
                            break;
                        }
                    case "Dijkstra":
                        {
                            if(selectedVertex != null && selectedVertex != vc.Vertex)
                            {
                                thread = new System.Threading.Thread(x => Dijkstra.CalculateDijkstra(selectedVertex, vertex, graph));                               
                            }
                            else
                            {
                                selectedVertex = (GraphData.Model.DataVertex)vc.Vertex;
                                MessageBox.Show(Properties.Language.ChooseNextVertex, Properties.Language.Caption);
                            }
                            break;
                        }
                    case "POC":
                        {
                            System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource();
                            thread = new System.Threading.Thread(x => POC.CalculatePOC(graph));
                            break;
                        }

                    default:
                        {
                            MessageBox.Show(Properties.Language.ChooseAlgorithmMessage, Properties.Language.Caption);
                            break;
                        }
                }
                if (thread != null)
                {
                    var sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    thread.Start();
                    thread.Join();
                    sw.Stop();
                    ChatBox.AppendText(Properties.Language.SendMessageYou + algorithmResult + "\n\n");
                    chatCount += 1;
                    ChatsScrollView.ScrollToBottom();
                    SocketConnector.SendMessage(algorithmResult);
                    System.Windows.MessageBox.Show(Properties.Language.AlgorithmTimeMessage + sw.Elapsed.TotalSeconds + Properties.Language.Seconds, Properties.Language.Caption);
                }
            }
        } 
        public async void DrawAlgorithm()
        {
            isDrawing = true;
            FixLabelsAndArrows();
            try
            {
                foreach (var item in algorithmEdgesList)
                {
                    await Task.Delay(AlgorithmHelper.AlgorithmTime, source);
                    if(isDrawing == false)
                    {
                        FixLabelsAndArrows();
                        return;
                    }
                    foreach (var edge in GraphArea.EdgesList.Keys)
                    {
                        if (item.Source == edge.Target && item.Target == edge.Source)
                        {
                            edge.EdgeBrush = Brushes.Red;

                            break;
                        }
                    }
                    //item.Source.VertexColor = Brushes.Red;
                    //item.Target.VertexColor = Brushes.Red;
                    item.EdgeBrush = Brushes.Red;
                    item.ArrowBrush = Brushes.Red;
                }
                algorithmsBtn.IsEnabled = true;
                showAnimationText.Text = Properties.Language.ShowAlgorithm;
                showAnimatonButton.ToolTip = Properties.Language.ShowAlgorithmTooltip;
                isDrawing = false;
            }
            catch
            {
                FixLabelsAndArrows();
            }
        }
    }
}