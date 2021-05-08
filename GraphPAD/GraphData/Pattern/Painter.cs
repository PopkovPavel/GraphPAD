using GraphPAD.GraphData.Model;
using QuickGraph;
using System.Collections.Generic;

namespace GraphPAD.GraphData.Pattern
{
    internal abstract class Painter
    {
        public static GraphZone GraphZone { get; set; }

        internal BidirectionalGraph<DataVertex, DataEdge> Graph { get; set; }

        public abstract void Draw();
    }

    internal class GraphPainter : Painter
    {
        public GraphPainter(BidirectionalGraph<DataVertex, DataEdge> gr)
        {
            Graph = new BidirectionalGraph<DataVertex, DataEdge>();
            var edges = new List<DataEdge>(gr.Edges);
            var vertices = new List<DataVertex>(gr.Vertices);
            foreach (var v in vertices)
            {
                Graph.AddVertex(v);
            }
            foreach (var e in edges)
            {
                Graph.AddEdge(e);
            }
        }

        public override void Draw()
        {
            GraphZone.GenerateGraph(Graph);
        }
        public string SendGraph()
        {
            string test = null;
            foreach (DataEdge dataEdge in GraphZone.EdgesList.Keys)
            {
 //               dataEdge.Source.
            }
            return test;
        }

    }
}
