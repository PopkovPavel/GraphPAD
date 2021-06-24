using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPAD.GraphData.Algorithms
{
    public abstract class AlgorithmHelper
    {
        public static int AlgorithmTime = 1500;
        public static int VertexCount = 0;
        public static string ChoosedAlgorithm = "";
        //public static void WaitUntilAlghoritmEnd(System.Threading.Thread thread)
        //{
        //    System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
        //    myStopwatch.Start();
        //    while (thread.IsAlive)
        //    {
        //        System.Windows.MessageBox.Show(thread.ThreadState.ToString());
        //    }
            
        //    myStopwatch.Stop();
        //    System.Windows.MessageBox.Show(thread.ThreadState.ToString());
        //    TimeSpan ts = myStopwatch.Elapsed;
        //    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        //    ts.Hours, ts.Minutes, ts.Seconds,
        //    ts.Milliseconds / 10);
        //    System.Windows.MessageBox.Show($"Алгоритм закончил выполнил свою работу за {elapsedTime}");
        //}
        /// <summary>
        /// Сгенерировать ориентированный случайно связный граф
        /// </summary>
        /// <param name="vertices">Количество вершин</param>
        /// <param name="edges">Количество ребер</param>
        /// <returns>Матрица вида int[,]</returns>
        public static int[,] GenerateOrientedRandomlyConnectedGraph(int vertices, int edges)
        {
            int[,] Data2D = null;
            Random rnd = new Random();

            if (!(vertices < 1 || vertices > 999 || edges < vertices - 1 || edges > vertices * (vertices - 1)))
            {
                Data2D = new int[vertices, vertices];
                for (int i = vertices - 1; i > 0; i--)
                {
                    Data2D[rnd.Next(0, i), i] = 1;
                }
                int temp = 0;
                int count = edges - vertices + 1;
                while (count != 0)
                {
                    int i = rnd.Next(0, vertices);
                    int j = rnd.Next(0, vertices);
                    if (Data2D[i, j] == 0 && (i != j))
                    {
                        //если нажата кнопка "без веса"
                        //if (true)
                        //{
                        Data2D[i, j] = 1;

                        //}
                        //else
                        //{
                        //    Data2D[i, j] = rnd.Next(0, 30);
                        //}
                        count--;
                    }
                    else
                    {
                        if (temp == 3)
                        {
                            bool tempFlag = false;
                            for (int ik = 0; ik < vertices - 1; ik++)
                            {
                                if (tempFlag)
                                {
                                    break;
                                }
                                for (int jk = 0; jk < vertices - 1; jk++)
                                {
                                    if (Data2D[ik, jk] == 0 && (ik != jk))
                                    {

                                        //если нажата кнопка "без веса"
                                        //if (true)
                                        //{
                                        Data2D[ik, jk] = 1;
                                        //}
                                        //else
                                        //{
                                        //    Data2D[i, j] = rnd.Next(0, 30);
                                        //}
                                        count--;
                                        tempFlag = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            temp += 1;
                        }
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show(Properties.Language.EdgesCountErrorMessage, Properties.Language.Caption);
            }
            return Data2D;
        }
        /// <summary>
        /// Сгенерировать ориентированное случайное дерево
        /// </summary>
        /// <param name="vertices">Количество вершин</param>
        /// <returns>Матрица вида int[,]</returns>
        public static int[,] GenerateOrientedRandomTree(int vertices)
        {
            int[,] Data2D = null;
            Random rnd = new Random();
            if (!(vertices < 1 || vertices > 999))
            {
                Data2D = new int[vertices, vertices];
                for (int i = vertices - 1; i > 0; i--)
                {
                    /**
                     * (rand() % i) - случайное число в множестве [0, i)
                     * i-тый столбец
                     */
                    rnd.Next(1, i);
                    Data2D[rnd.Next(0, i), i] = 1;                 
                }

            }
            else
            {
                System.Windows.MessageBox.Show(Properties.Language.EdgesCountMessage, Properties.Language.Caption);
            }
            return Data2D;
        }
        /// <summary>
        /// Сгенерировать неориентированный случайно связный граф
        /// </summary>
        /// <param name="vertices">Количество вершин</param>
        /// <param name="edges">Количество ребер</param>
        /// <returns>Матрица вида int[,]</returns>
        public static int[,] GenerateNonOrientedRandomlyConnectedGraph(int vertices, int edges)
        {
            int[,] Data2D = null;
            Random rnd = new Random();

            if (!(vertices < 1 || vertices > 999 || edges < vertices - 1 || edges > vertices * (vertices-1)/2))
            {
                Data2D = new int[vertices, vertices];
                for (int i = vertices - 1; i > 0; i--)
                {
                    var tempRnd = rnd.Next(0, i);
                    Data2D[tempRnd, i] = 1;
                    Data2D[i, tempRnd] = 1;

                }
                int temp = 0;
                int count = edges - vertices + 1;
                while (count != 0)
                {
                    int i = rnd.Next(0, vertices);
                    int j = rnd.Next(0, vertices);
                    if (Data2D[i, j] == 0 && (i != j))
                    {
                        Data2D[i, j] = 1;
                        Data2D[j, i] = 1;
                        count--;
                    }
                    else
                    {
                        if (temp == 3)
                        {
                            bool tempFlag = false;
                            for (int ik = 0; ik < vertices - 1; ik++)
                            {
                                if (tempFlag)
                                {
                                    break;
                                }
                                for (int jk = 0; jk < vertices - 1; jk++)
                                {
                                    if (Data2D[ik, jk] == 0 && (ik != jk))
                                    {
                                        Data2D[ik, jk] = 1;
                                        Data2D[jk, ik] = 1;

                                        count--;
                                        tempFlag = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            temp += 1;
                        }
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show(Properties.Language.EdgesCountErrorMessage, Properties.Language.Caption);
            }
            return Data2D;
        }
        /// <summary>
        /// Сгенерировать неориентированный случайное дерево
        /// </summary>
        /// <param name="vertices">Количество вершин</param>
        /// <returns>Матрица вида int[,]</returns>
        public static int[,] GenerateNonOrientedRandomTree(int vertices)
        {
            int[,] Data2D = null;
            Data2D = new int[vertices, vertices];
            Random rnd = new Random();
            if (!(vertices < 1 || vertices > 999))
            {
                for (int i = vertices - 1; i > 0; i--)
                {
                    /**
                     * (rand() % i) - случайное число в множестве [0, i)
                     * i-тый столбец
                     */
                    var temp = rnd.Next(0, i);
                    Data2D[temp, i] = 1;
                    Data2D[i, temp] = 1;
                }

            }
            else
            {
                System.Windows.MessageBox.Show(Properties.Language.EdgesCountMessage, Properties.Language.Caption);
            }
            return Data2D;
        }
    }
}
