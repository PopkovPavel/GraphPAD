using GraphX.PCL.Common.Models;
using System;

namespace GraphPAD.GraphData.Model
{
    public class DataVertex : VertexBase, ICloneable
    {
        public string Text { get; set; }

        public string Color { get; set; }

        /// <summary>
        /// Степень
        /// </summary>
        public int E { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Color))
                return Text + "(" + Color + ")";
            return Text;
        }

        public object Clone()
        {
            return new DataVertex
            {
                Angle = this.Angle,
                Color = this.Color,
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

        }

        public DataVertex(string text = "")
        {
            Text = string.IsNullOrEmpty(text) ? "New Vertex" : text;
        }


    }
}
