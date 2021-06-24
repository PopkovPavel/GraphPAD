using GraphPAD.GraphData.Model;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphPAD.GraphData.Algorithms
{
    public class HelpClass
    {   
        public DataVertex Vertex1 { get; set; }      
        public DataVertex Vertex2 { get; set; }
        public List<double> Reliability { get; set; }
        public double OverallReliability;
        public double CalculateReliability()
        {
            double Result = 1;
            foreach(double rel in Reliability)
            {
                Result *= (1 - rel);
            }
            //Result = Result;
           // MessageBox.Show($"{Vertex1.Text} -- {Vertex2.Text} == {Result}");
            return Result;
        }
        public HelpClass(DataVertex v1, DataVertex v2, List<double> list)
        {
            Vertex1 = v1;
            Vertex2 = v2;
            Reliability = list;
        }
        
    }
    /// <summary>
    /// probability of connectivity (вообще reliability, ну да ладно) (Вероятность связности случайного графа)
    /// </summary>
    public abstract class POC
    {
        private static double POCUtil(BidirectionalGraph<DataVertex, DataEdge> graph)
        {
            HashSet<DataVertex> visitedVertices = new HashSet<DataVertex>();
            foreach (var vertexTemp in graph.Vertices)
            {
                bool temp = false;
                foreach (var edge in graph.Edges)
                {
                    if (edge.Source == vertexTemp)
                    {
                        temp = true;
                    }
                    if (edge.Target == vertexTemp)
                    {
                        temp = true;
                    }
                }
                if (!temp)
                {
                    return 0;
                }
            
        
        
            }

            if (graph.Vertices.Count() == 4)
            {
                List<HelpClass> helpClasses = new List<HelpClass>();
                HashSet<DataEdge> visitedEdges = new HashSet<DataEdge>();
                foreach (var edge in graph.Edges)
                {                        
                    foreach(var edge2 in graph.Edges)
                    {
                        if(edge.Source==edge2.Target 
                            && edge.Target == edge2.Source
                            && edge.Reliability == edge2.Reliability 
                            &&!visitedEdges.Contains(edge2)
                            &&!visitedEdges.Contains(edge))
                        {
                            visitedEdges.Add(edge);
                            visitedEdges.Add(edge2);
                            var tempHelp1 = helpClasses.Find(x => x.Vertex1 == edge.Source && x.Vertex2 == edge.Target);
                            var tempHelp2 = helpClasses.Find(x => x.Vertex1 == edge.Target && x.Vertex2 == edge.Source);
                            if (tempHelp1 != null)
                            {
                                tempHelp1.Reliability.Add(edge.Reliability);
                            } else if(tempHelp2 != null)
                            {
                                tempHelp2.Reliability.Add(edge.Reliability);
                            } else
                            {
                                var probab = new List<double>();
                                probab.Add(edge.Reliability);
                                helpClasses.Add(new HelpClass(edge.Source, edge.Target, probab));
                            }             
                        }
                    }
                }
                try
                {
                    var a = 1 - helpClasses[0].CalculateReliability();
                    var b = 1 - helpClasses[1].CalculateReliability();
                    var c = 1 - helpClasses[2].CalculateReliability();
                    var d = 1 - helpClasses[3].CalculateReliability();
                    var e = 1 - helpClasses[4].CalculateReliability();
                    var f = 1 - helpClasses[5].CalculateReliability();
                    var e2 = helpClasses[4].CalculateReliability(); 
                    var f2 = helpClasses[5].CalculateReliability();
                    //a=0,b=1,c=2,d=3,e=4,f=5
                    double result =
                        6 * a * b * c * d * e * f
                        + (a * b * e)
                        + (a * d * f)
                        + (b * c * f)
                        + (c * d * e)
                        - 2 * (b * d * e * f * (a + c - 0.5) 
                        + a * c * e * f * (b + d - 0.5) 
                        + a * b * c * d * (e2 + f2 - 0.5));
                    return result;
                }
                catch (Exception e)
                {
                    //MessageBox.Show("все плохо");
                    return 0;
                }
            }
            else if (graph.Vertices.Count() > 4)
            {
                foreach (var edge in graph.Edges)
                {
                    foreach(var edge2 in graph.Edges)
                    {
                        if(edge.Source == edge2.Source && edge.Target == edge2.Target && edge != edge2)
                        {
                            visitedVertices.Add(edge.Source);
                        }
                    }
                }
                DataEdge edgeToContratation = new DataEdge();
                foreach (var edge in graph.Edges)
                {
                    if (!visitedVertices.Contains(edge.Source))
                    {
                        edgeToContratation = edge;
                        break;
                    }
                }

                var graphWithoutEdge = new BidirectionalGraph<DataVertex, DataEdge>();
                var graphWithContratationEdge = new BidirectionalGraph<DataVertex, DataEdge>();
                foreach (var vertex in graph.Vertices)
                {
                    graphWithoutEdge.AddVertex(vertex);
                }
                foreach (var edge in graph.Edges)
                {
                    if (edge.Source == edgeToContratation.Source && edge.Target == edgeToContratation.Target
                    || edge.Target == edgeToContratation.Source && edge.Source == edgeToContratation.Target)
                    {
                    }
                    else
                    {
                            graphWithoutEdge.AddEdge(edge);
                    }
                }

                foreach (var vertex in graph.Vertices)
                {
                    if (vertex != edgeToContratation.Source)
                    {
                        graphWithContratationEdge.AddVertex(vertex);
                    }
                }
                foreach (var edge in graph.Edges)
                {
                    if (edge.Target == edgeToContratation.Source 
                        && edge != edgeToContratation
                        && edge.Source != edgeToContratation.Target)
                    {
                        graphWithContratationEdge.AddEdge(new DataEdge(
                                    source: graphWithContratationEdge.Vertices.ToList().Find(x => x.Text == edge.Source.Text),
                                    target: graphWithContratationEdge.Vertices.ToList().Find(x => x.Text == edgeToContratation.Target.Text),
                                    weight: edgeToContratation.Weight,
                                    colorBrush: Brushes.Black,
                                    reliability: edgeToContratation.Reliability
                                    ));
                        
                    } else if(edgeToContratation.Source == edge.Source && edge != edgeToContratation
                        && edge.Target != edgeToContratation.Target)
                    {
                        var edges = new List<DataEdge>();
                        edges.Add(new DataEdge(
                                    source: graphWithContratationEdge.Vertices.ToList().Find(x => x.Text == edgeToContratation.Target.Text),
                                    target: graphWithContratationEdge.Vertices.ToList().Find(x => x.Text == edge.Target.Text),
                                    weight: edgeToContratation.Weight,
                                    colorBrush: Brushes.Black,
                                    reliability: edgeToContratation.Reliability
                                    ));
                        foreach(var edge2 in edges)
                        {
                        graphWithContratationEdge.AddEdge(edge2);
                        }
                    } else if (
                        (edge.Target == edgeToContratation.Target 
                        && edge.Source == edgeToContratation.Source)
                        ||(edge.Source == edgeToContratation.Target 
                        && edge.Target==edgeToContratation.Source))
                        {

                        }else
                        graphWithContratationEdge.AddEdge(edge);
                }
                return (edgeToContratation.Reliability*POCUtil(graphWithContratationEdge)
                +(1-edgeToContratation.Reliability)*POCUtil(graphWithoutEdge));
            } else
            {
                MessageBox.Show("Если ты это видишь, то ты в дегьме");
                return 0;
            }


        }

        public static void CalculatePOC(GraphZone graph)
        {
            MainPage.algorithmEdgesList.Clear();
            try
            {
                MainPage.algorithmResult = "\n" + Properties.Language.ResultPOC + ":";
                HashSet<DataVertex> visitedList = new HashSet<DataVertex>();

                var Graph = new BidirectionalGraph<DataVertex, DataEdge>();
                foreach (var vertex in graph.VertexList.Keys)
                {
                    Graph.AddVertex(vertex);
                }
                foreach (var edge in graph.EdgesList.Keys)
                {
                    Graph.AddEdge(edge);
                }
                var temp = POCUtil(Graph);
                MainPage.algorithmResult += temp;
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "Поток находился в процессе прерывания.")
                {
                    
                } 
                System.Windows.MessageBox.Show(ex.ToString());

            }
        }
    }
}
