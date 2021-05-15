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
        public async void DrawAlgorithm()
        {
            foreach(var item in algorithmEdgesList)
            {
                await Task.Delay(500, source);
                item.EdgeBrush = Brushes.Red;
                item.ArrowBrush = Brushes.Red;

            }
        }
    }
}