using GraphPAD.GraphData.Model;
using QuickGraph;
using System.Collections.Generic;


namespace GraphPAD.GraphData.Pattern
{
    abstract class Builder
    {
        public abstract void BuildMatrix(int[,] m);
        public abstract void BuildGraph();

        public abstract BidirectionalGraph<DataVertex, DataEdge> GetResult();
    }

    class Director
    {
        Builder builder;
        public Director(Builder builder)
        {
            this.builder = builder;
        }
        public void Construct(int[,] m)
        {

            builder.BuildMatrix(m);
            builder.BuildGraph();

        }
    }

    class GraphBuilder : Builder
    {
        public int[,] GraphMatrix { get; set; }
        public BidirectionalGraph<DataVertex, DataEdge> Graph { get; set; }

        /// <summary>
        /// Конструирование матрицы
        /// </summary>
        /// <param name="m"></param>
        public override void BuildMatrix(int[,] m)
        {

            GraphMatrix = new int[,] { };
            GraphMatrix = (int[,])m.Clone();

        }

        /// <summary>
        /// Конструирование графа
        /// </summary>
        public override void BuildGraph()
        {
            var size = GraphMatrix.GetLength(1);
            Graph = new BidirectionalGraph<DataVertex, DataEdge>();
            var vertices = new List<DataVertex>();
            var edges = new List<DataEdge>();

            for (var i = 0; i < size; i++)
            {
                var vertex = new DataVertex((i + 1).ToString()) { ID = i };
                vertices.Add(vertex);
            }

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    if (i != j)
                    {
                        var weight = GraphMatrix[i, j];
                        if (weight > 0)
                        {
                            edges.Add(new DataEdge(vertices[i], vertices[j], weight));
                            vertices[i].E++;
                            vertices[j].E++;
                        }
                    }
                }
            }

            foreach (var v in vertices)
            {
                v.E /= 2;
            }

            foreach (var v in vertices)
            {
                Graph.AddVertex(v);
            }

            foreach (var e in edges)
            {
                Graph.AddEdge(e);
            }
        }

        /// <summary>
        /// Возвращаем готовый продукт
        /// </summary>
        /// <returns></returns>
        public override BidirectionalGraph<DataVertex, DataEdge> GetResult()
        {
            return Graph;
        }
    }
}
