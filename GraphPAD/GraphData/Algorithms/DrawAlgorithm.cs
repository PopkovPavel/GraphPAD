using GraphPAD.GraphData.Algorithms;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphPAD
{
    
    public partial class MainPage : Window
    {
        
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