using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphPAD
{
    public partial class MainPage : Window
    {

        public async void CalculateDjkstra() 
        {
            try
            {


                foreach (var edgePair in GraphArea.EdgesList)
                {
                    if (flagNegr == true) return;
                    //  edgePair
                    // GraphArea.RelayoutGraph();
                    //var temp = this.GraphArea.EdgesList[edgePair.Key].Edge;// = Brushes.Red; 
                    // edgePair.Key.EdgeBrush = Brushes.Red;
                    edgePair.Key.EdgeBrush = Brushes.Red;
                    edgePair.Key.ArrowBrush = Brushes.Yellow;
                    await Task.Delay(500, source);
                    //  GraphArea.RelayoutGraph();
                    var temp = edgePair.Key;
                    //GraphArea.bindi
                }
            }
            catch
            {

            }
            //    GraphArea.
        }

    }
}
