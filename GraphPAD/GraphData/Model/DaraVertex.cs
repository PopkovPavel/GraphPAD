using GraphX.PCL.Common.Models;
using System;
using System.Reflection;
using System.Windows.Media;

namespace GraphPAD.GraphData.Model
{
    public class DataVertex : VertexBase, ICloneable
    {
        public string Text { get; set; }

        public SolidColorBrush VertexColor { get; set; }

        /// <summary>
        /// Степень
        /// </summary>
        public int E { get; set; }

        public override string ToString()
        {           
            return Text;
        }

        public object Clone()
        {
            return new DataVertex
            {
                Angle = this.Angle,
                VertexColor = this.VertexColor,
                E = this.E,
                GroupId = this.GroupId,
                ID = this.ID,
                SkipProcessing = this.SkipProcessing,
                Text = this.Text
            };
        }
        /// <summary>
        /// Default constructor for this class
        /// (required for serialization).
        /// </summary>
        public DataVertex() : this(string.Empty)
        {
            Random rnd = new Random();
            byte c1 = (byte)rnd.Next(0,160);
            byte c2 = (byte)rnd.Next(0, 160);
            byte c3 = (byte)rnd.Next(0, 160);

            VertexColor = new SolidColorBrush(Color.FromRgb(c1, c2, c3));
        }

        public DataVertex(string text = "", SolidColorBrush solidColorBrush = null)
        {
            VertexColor = solidColorBrush;
            Text = string.IsNullOrEmpty(text) ? "New Vertex" : text;
        }


    }
}
