using GraphX.Measure;
using GraphX.PCL.Common.Models;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace GraphPAD.GraphData.Model
{
    [Serializable]
    public class DataEdge : EdgeBase<DataVertex>, INotifyPropertyChanged
    {
        public override Point[] RoutingPoints { get; set; }

        public DataEdge(DataVertex source, DataVertex target, double weight = 1, Brush colorBrush = null)
            : base(source, target, weight)
        {
            if (colorBrush == null) ArrowBrush = Brushes.Black;
            ArrowBrush = (SolidColorBrush)colorBrush;
            Angle = 90;
            Text = weight.ToString();
        }

        public DataEdge()
            : base(null, null, 1)
        {
            ArrowBrush = Brushes.Black;
            Angle = 90; 
        }

        public bool ArrowTarget { get; set; }

        public double Angle { get; set; }
        public SolidColorBrush EdgeBrush { get; set; } = Brushes.Black;
        public SolidColorBrush ArrowBrush { get; set; } = Brushes.Black;

        /// <summary>
        /// Node main description (header)
        /// </summary>
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }
        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
