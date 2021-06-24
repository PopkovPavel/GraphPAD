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

        public DataEdge(DataVertex source, DataVertex target, double weight = 1, Brush colorBrush = null, Brush edgeBrush = null, double reliability = 1)
            : base(source, target, weight)
        {
            if (colorBrush == null)
            {
                ArrowBrush = Brushes.Black;
            } 
            else
            {
            ArrowBrush = (SolidColorBrush)colorBrush;
            }
            if (edgeBrush == null)
            {
                EdgeBrush = Brushes.Black;
            }
            else
            {
                EdgeBrush = (SolidColorBrush)edgeBrush;
            }
            Angle = 90;
            Reliability = reliability;
            if (Reliability != 1)
            {
                //Text = "W: " + weight.ToString() 
                //  + "\nR:" + Math.Round(Probability,3).ToString();
                Text = "P:"+Math.Round(Reliability, 5).ToString();
            }
            else
            {
                Text = "W:"+weight.ToString();
            }
        }

        public DataEdge()
            : base(null, null, 1)
        {
            ArrowBrush = Brushes.Black;
            EdgeBrush = Brushes.Black;
            Angle = 90;
            Reliability = 1;
        }

        public bool ArrowTarget { get; set; }

        public double Angle { get; set; }
        private SolidColorBrush _edgeBrush;
        private SolidColorBrush _arrowBrush;
        private double _probability = 1;
        public double Reliability
        {
            get
            {
                return _probability;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    _probability = 1;
                }
                else
                {
                    _probability = value;
                }
                OnPropertyChanged("Probability");
            }
        }
        public SolidColorBrush EdgeBrush { 
            get 
            {
                return _edgeBrush;
            } 
            set 
            { 
                _edgeBrush = value;
                OnPropertyChanged("EdgeBrush");
            } 
        }
        public SolidColorBrush ArrowBrush
        {
            get
            {
                return _arrowBrush;
            }
            set
            {
                _arrowBrush = value;
                OnPropertyChanged("ArrowBrush");
            }
        }

        /// <summary>
        /// Node main description (header)
        /// </summary>
        private string _text = "0";
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
