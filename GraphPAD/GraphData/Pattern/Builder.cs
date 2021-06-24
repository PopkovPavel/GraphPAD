using GraphPAD.GraphData.Model;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Windows.Media;

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
            //Random rnd = new Random();
            GraphMatrix = new int[,] { };
           // ProbMatrix = new double[,] { };
            GraphMatrix = (int[,])m.Clone();
            //for (var i = 0; i < GraphMatrix.GetLength(1); i++)
            //{
            //    for (var j = 0; j < i; j++)
            //    {
            //        if (GraphMatrix[i, j] != 0)
            //        {
            //            var temp = rnd.NextDouble();
            //            ProbMatrix[i, j] = temp;
            //            if (GraphMatrix[j, i] != 0)
            //            {
            //                ProbMatrix[j, i] = temp;
            //            }
            //        }
            //    }
            //}
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
            Random rnd = new Random();
            for (var i = 0; i < size; i++)
            {
                
                byte c1 = (byte)rnd.Next(33, 160);
                byte c2 = (byte)rnd.Next(33, 160);
                byte c3 = (byte)rnd.Next(33, 160);
                
                var VertexColor = new SolidColorBrush(Color.FromRgb(c1, c2, c3));
              
                var vertex = new DataVertex((i + 1).ToString(), VertexColor) { ID = i };
                GraphData.Algorithms.AlgorithmHelper.VertexCount += 1;
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
                            var randomReliability = rnd.NextDouble();

                            randomReliability = edges.Find(x =>x.Source == vertices[j] && x.Target==vertices[i])?.Reliability ?? randomReliability;
                            randomReliability = edges.Find(x => x.Source == vertices[i] && x.Target == vertices[j])?.Reliability ?? randomReliability;
                            
                            edges.Add(new DataEdge(
                                source: vertices[i],
                                target: vertices[j], 
                                weight: weight,
                                colorBrush: Brushes.Black,
                                reliability: randomReliability
                                ));
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
