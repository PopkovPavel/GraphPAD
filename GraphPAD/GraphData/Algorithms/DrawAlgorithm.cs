using GraphPAD.GraphData.Algorithms;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphPAD
{
    
    public partial class MainPage : Window
    {
        private void StartAlgorithm(GraphX.Controls.VertexControl vc)
        {
            FixLabelsAndArrows();
            if (isAlgorithmsOn)
            {
                switch (AlgorithmHelper.ChoosedAlgorithm)
                {
                    case "DFS":
                        {
                            CalculateDFS((GraphData.Model.DataVertex)vc.Vertex);
                            break;
                        }
                    case "Dijkstra":
                        {
                            if(selectedVertex != null && selectedVertex != vc.Vertex)
                            {
                                CalculateDijkstra((GraphData.Model.DataVertex)vc.Vertex);
                                
                            }
                            else
                            {
                                selectedVertex = (GraphData.Model.DataVertex)vc.Vertex;
                                MessageBox.Show("Теперь выберите вторую вершину");
                            }
                            break;
                        }
                    case "": 
                        {
                            MessageBox.Show("Выберите алгоритм");
                            break; 
                        }
                    default:
                        {
                            MessageBox.Show("Выберите алгоритм");
                            break;
                        }
                }
            }
        }
        public async void DrawAlgorithm()
        {
            FixLabelsAndArrows();
            try
            {

                foreach (var item in algorithmEdgesList)
                {
                    await Task.Delay(AlgorithmHelper.AlgorithmTime, source);
                    item.EdgeBrush = Brushes.Red;
                    item.ArrowBrush = Brushes.Red;

                }
                chatCount += 1;
                ChatsScrollView.ScrollToBottom();
                ChatBox.AppendText($"Вы: {algorithmResult}\n\n");
                SocketConnector.SendMessage(algorithmResult);
            }
            catch
            {
                FixLabelsAndArrows();
            }
        }
    }
}