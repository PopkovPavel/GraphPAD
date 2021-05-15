using GraphPAD.GraphData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPAD.GraphData.Algorithms
{
    class MyTuple
    {
        public DataVertex Vertex { get; set; }
        public bool Visited { get; set; }
        public MyTuple(DataVertex vertex, bool visited)
        {
            Vertex = vertex;
            Visited = visited;
        }

    }
}
