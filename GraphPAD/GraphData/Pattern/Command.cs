using GraphPAD.GraphData.Model;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphPAD.GraphData.Pattern
{
    internal abstract class Command
    {
        public static BidirectionalGraph<DataVertex, DataEdge> Graph;
        public Painter Creator;

        public abstract void Execute();
        public abstract void UnExecute();
    }

    internal class DeleteCliqueCommand : Command
    {
        public int deleteMode;
        public BidirectionalGraph<DataVertex, DataEdge> OldGraph;
        //public List<Clique> OldCliques;
        public static StackPanel Sp;

        public DeleteCliqueCommand(int mode) { deleteMode = mode; }

        public override void Execute()
        {
            OldGraph = new BidirectionalGraph<DataVertex, DataEdge>();
            var edges = new List<DataEdge>(Graph.Edges);
            var vertices = new List<DataVertex>(Graph.Vertices);
            foreach (var v in vertices)
            {
                OldGraph.AddVertex(v);
            }
            foreach (var e in edges)
            {
                OldGraph.AddEdge(e);
            }

            //SelectDeleteMode();

            Creator = new GraphPainter(Graph);
            Creator.Draw();
        }

        public override void UnExecute()
        {
            //Cliques = new List<Clique>(OldCliques);
            Graph = new BidirectionalGraph<DataVertex, DataEdge>();
            var edges = new List<DataEdge>(OldGraph.Edges);
            var vertices = new List<DataVertex>(OldGraph.Vertices);
            foreach (var v in vertices)
            {
                Graph.AddVertex(v);
            }
            foreach (var e in edges)
            {
                Graph.AddEdge(e);
            }

            Creator = new GraphPainter(Graph);
            Creator.Draw();

            Sp.Children.RemoveAt(Sp.Children.Count - 1);
        }

        #region Воспомогательные функции

        /// <summary>
        /// Поиск ксех клик
        /// </summary>
        //public void FindAllCliques()
        //{
        //    var bk = new BronKerbosch(Graph.Edges.ToList());

        //    bk.FindClique(new Clique(), new List<DataVertex>(Graph.Vertices.ToList()), new List<DataVertex>());
        //    Cliques = bk.RemoveDuplicatedLists();
        //}

        /// <summary>
        /// Вывод найденных клик
        /// </summary>
        //public void ShowFoundCliques()
        //{
        //    var sbLine = new StringBuilder();
        //    foreach (var clique in Cliques)
        //    {
        //        sbLine.AppendLine(String.Format(CliqueToString(clique)));
        //    }
        //    MessageBox.Show(sbLine.ToString(), "Найденные клики");
        //}

        /// <summary>
        /// Последовательное или параллельное удаление
        /// </summary>
        //public void SelectDeleteMode()
        //{
        //    FindAllCliques();

        //    //проверка режима удаления
        //    if (deleteMode == 2)
        //    {
        //        GroupCliques();
        //    }

        //    ShowFoundCliques();
        //    DeleteClique();
        //}

        /// <summary>
        /// Удаление клики
        /// </summary>
        //public void DeleteClique()
        //{
        //    OldCliques = new List<Clique>(Cliques);

        //    var c = Cliques.First();
        //    Cliques.Remove(c);

        //    ShowDeletedClique(c);

        //    c.RemoveVertices("0");

        //    foreach (var vertex in c)
        //    {
        //        //поиск всех ребер, где вершина является началом и концом
        //        var edgesBegin = Graph.Edges.ToList().FindAll(x => x.Source.ID == vertex.ID);
        //        var edgesEnd = Graph.Edges.ToList().FindAll(x => x.Target.ID == vertex.ID);
        //        var neighbourVertices = new List<DataVertex>();

        //        //получили всех соседей удаляемой вершины
        //        foreach (var edge in edgesBegin)
        //        {
        //            neighbourVertices.Add(edge.Target);
        //        }

        //        //удаляем вершину
        //        Graph.RemoveVertex(vertex);

        //        for (var i = 0; i != neighbourVertices.Count; i++)
        //        {
        //            for (var j = i + 1; j != neighbourVertices.Count; j++)
        //            {
        //                var edge = new DataEdge(neighbourVertices[i], neighbourVertices[j]);
        //                if (!EdgeExists(edge))
        //                    Graph.AddEdge(edge);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Группировка несвязанных клик
        /// </summary>
        //public void GroupCliques()
        //{
        //    for (var i = 0; i < Cliques.Count; i++)
        //    {
        //        for (var j = i; j < Cliques.Count; j++)
        //        {
        //            if (!Cliques[i].Intersect(Cliques[j]).Any())
        //            {
        //                Cliques[i].Add(new DataVertex { ID = 0, Text = "0" });
        //                Cliques[i].AddClique(Cliques[j]);
        //                Cliques[j] = null;
        //            }
        //        }
        //        RemoveNulls();
        //    }
        //}

        /// <summary>
        /// Удалить пустые кликки
        /// </summary>
        //public void RemoveNulls()
        //{
        //    Cliques.RemoveAll(x => x == null);
        //}

        /// <summary>
        /// Отобразить удаленную клику
        /// </summary>
        /// <param name="clique"></param>
        //public void ShowDeletedClique(Clique clique)
        //{
        //    var gb = new GroupBox
        //    {
        //        Header = CliqueToString(clique),
        //        BorderBrush = Brushes.LightGray,
        //        BorderThickness = new Thickness(0.7)
        //    };

        //    var sv = new ScrollViewer
        //    {
        //        VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
        //        HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
        //    };

        //    var dp = new DockPanel
        //    {
        //        Background = Brushes.Transparent,
        //        HorizontalAlignment = HorizontalAlignment.Left
        //    };
        //    foreach (var vertex in clique)
        //    {
        //        var grid = new Grid
        //        {
        //            Margin = new Thickness(5),
        //            Background = Brushes.Transparent
        //        };
        //        if (vertex.Text != "0")
        //        {
        //            grid.Children.Add(new Ellipse
        //            {
        //                Height = 50,
        //                Width = 50,
        //                Stroke = Brushes.Black,
        //                StrokeThickness = 2
        //            });
        //            grid.Children.Add(new TextBlock
        //            {
        //                Text = vertex.Text,
        //                HorizontalAlignment = HorizontalAlignment.Center,
        //                VerticalAlignment = VerticalAlignment.Center
        //            });
        //        }
        //        else
        //        {
        //            grid.Children.Add(new Ellipse
        //            {
        //                Height = 50,
        //                Width = 1,
        //                Stroke = Brushes.Black,
        //                StrokeThickness = 2
        //            });
        //        }
        //        dp.Children.Add(grid);
        //        sv.Content = dp;
        //    }
        //    gb.Content = sv;
        //    Sp.Children.Add(gb);
        //}

        /// <summary>
        /// Показать клику в виде строки
        /// </summary>
        /// <param name="cliqueList"></param>
        /// <returns></returns>
        //public string CliqueToString(Clique cliqueList)
        //{
        //    var sbClique = new StringBuilder("(");
        //    foreach (var ver in cliqueList)
        //    {
        //        if (ver.Text == "0")
        //        {
        //            sbClique.Append(") U (");
        //        }
        //        else
        //            sbClique.Append($" {ver.Text} ");
        //    }
        //    sbClique.Append(")");

        //    return $"Клика: {sbClique}";
        //}

        /// <summary>
        /// Соединения между вершиной и всеми вершинами из множества
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool HasEdges(DataVertex vertex, List<DataVertex> list)
        {
            //соединена ли вершина со всеми кандидатами
            var success = false;
            foreach (var v in list)
            {
                if (Graph.Edges.All(x => (x.Source.ID == vertex.ID && x.Target.ID == v.ID) || (x.Source.ID == v.ID && x.Target.ID == vertex.ID)))
                {
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Наличие ребра в графе
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool EdgeExists(DataEdge edge)
        {
            var success = Graph.Edges.Any(x => (x.Source.ID == edge.Source.ID && x.Target.ID == edge.Target.ID) || (x.Source.ID == edge.Target.ID && x.Target.ID == edge.Source.ID));
            return success;
        }

        #endregion
    }
}
