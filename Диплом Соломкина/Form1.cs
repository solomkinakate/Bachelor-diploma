using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RestrictedGraph
{   
    public partial class MainForm : Form
    {       
        public MainForm()
        {
            InitializeComponent();
            radioButtonBarrier.Select();
            radioButtonShortestPath.Select();
        }
        
        public void Go(object sender, EventArgs e)
        {
            if (!File.Exists(inFile)) {
                MessageBox.Show("Выберите файл и нажмите Go", "Файл не выбран", MessageBoxButtons.OK);
                inputFileButton_Click(this, null);
                return;
            }

            string[] data = File.ReadAllText(inFile).Split(new char[] { ' ', '\n' });
            
            
            DefineTaskAndLimits(data);
        }

        private void DefineTaskAndLimits (string [] data)
        {            
            if (radioButtonBarrier.Checked && radioButtonShortestPath.Checked)
                ComputeMinDistanceForBarrier(data);

            if (radioButtonVentil.Checked && radioButtonShortestPath.Checked)
                ComputeMinDistanceForVentil(data);

            if (radioButtonBarrier.Checked && RadioButtonRandomWalk.Checked)
                ComputeRandomWalkForBarrier(data);

            if (radioButtonVentil.Checked && RadioButtonRandomWalk.Checked)
                ComputeRandomWalkForVentil(data);

            if (radioButtonShluz.Checked && radioButtonShortestPath.Checked)
                ComputeMinDistanceForGate(data);

            if (radioButtonShluz.Checked && RadioButtonRandomWalk.Checked)
                ComputeRandomWalkForGate(data);
        }
        //barrier
        private void ComputeMinDistanceForBarrier(string[] data)
        {
            int vertices, edges, barrierJumpLevel;
            List<Edge<int, char>>[] graph;
            int vStart, vFinish;

            ReadGraphMinDistanceForBarrier(data, out vertices, out edges, out barrierJumpLevel, out graph, out vStart, out vFinish);
            
            int[] dist;
            Ancestor<char>[] prev;
            DoDejkstraAlgo<char>(graph, (barrierJumpLevel + 1) * vertices, vStart, out dist, out prev);

            string[] resultArray = InterpreteMinDistance<char>(vertices, barrierJumpLevel, vStart, vFinish, dist, prev);

            PrintResult(resultArray);
        }

        private void ReadGraphMinDistanceForBarrier(string[] data, out int vertices, out int edges, out int barrierJumpLevel, out  List<Edge<int, char>>[] graph, out int vStart, out int vFinish)
        {
            //vertices - количество вершин, edges - количество дуг, k - уровень барьерного перехода
            vertices = Convert.ToInt32(data[0]);
            edges = Convert.ToInt32(data[1]);
            barrierJumpLevel = Convert.ToInt32(data[2]);
            int expandedVertices = vertices * (barrierJumpLevel + 1);

            //развертка для графа (вершин в k + 1 раз больше, чем в исходном графе)
            //первые vertices вершин соответствуют исходному графу
            //последующие vertices * k  вершин - дубликаты vertices первых вершин
            //представляется в виде списка смежностей, для каждой вершины v graph(v) содержит
            //список ребер, исходящих из этой вершины, то есть список  Edge, 
            //где первый элемент — вершина, в которую ведёт ребро, а второй элемент — вес ребра и тертий элемент тип дуги
            graph = new List<Edge<int, char>>[expandedVertices];
            for (int i = 0; i < expandedVertices; ++i)
                graph[i] = new List<Edge<int, char>>();


            // в цикле считываем 4 значения. из файла уже считали три первых
            // и вне цикла считаем 2 последних
            for (int i = 3; i < data.Length - 2; i += 4)
            {
                //считываем дуги в формате: 
                //исходящая вершина, входящая вершина, вес дуги и тип дуги(нейтральная, положительная или барьерная)
                int vFrom       = Convert.ToInt32(data[i]) - 1;
                int vTo         = Convert.ToInt32(data[i + 1]) - 1;
                int weight      = Convert.ToInt32(data[i + 2]);
                char edgeType   = Convert.ToChar(data[i + 3][0]);

                switch (edgeType)
                {
                    case 'N':
                        for (int j = 0; j <= barrierJumpLevel; ++j)
                        {
                            graph[vFrom + j * vertices].Add(new Edge<int, char>(vTo + j * vertices, weight, edgeType));
                        }
                        break;
                    case '+':
                        for (int j = 0; j < barrierJumpLevel; ++j)
                        {
                            graph[vFrom + j * vertices].Add(new Edge<int, char>(vTo + (j + 1) * vertices, weight, edgeType));
                        }

                        graph[vFrom + vertices * barrierJumpLevel].Add(new Edge<int, char>(vTo + vertices * barrierJumpLevel, weight, edgeType));
                
                        break;
                    case 'b':
                        graph[vFrom + vertices * barrierJumpLevel].Add(new Edge<int, char>(vTo, weight, edgeType));
                
                        break;
                    default:
                        throw new Exception("Неверный символ типа дуги");
                       // break;
                }
            }

            vStart = Convert.ToInt32(data[data.Length - 2]) - 1; // стартовая вершина
            vFinish = Convert.ToInt32(data[data.Length - 1]) - 1; // финишная вершина

        }
        
        private void ComputeRandomWalkForBarrier(string[] data)
        {
            //формат ввода
            //количество дуг, количество вершин
            //граф
            //вершина старта, время
            List<Edge<double, char>>[] graph;
            int vertices, edges, k, vStart, time;

            ReadGraphRandomWalkForBarrier(data, out vertices, out edges, out k, out graph, out vStart, out time);

            List<double> prob = DoRandomWalkAlgo(graph, (k + 1) * vertices, vStart, time);

            string[] res = InterpreteRandomWalk(vertices, k, prob.ToArray());

            PrintResult(res);
        }
        
        private void ReadGraphRandomWalkForBarrier(string[] data, out int vertices, out int edges, out int k, out List<Edge<double, char>>[] graph, out int vStart, out int time)
        {
            //vertices - количество вершин, edges- количество дуг
            vertices = Convert.ToInt32(data[0]);
            edges = Convert.ToInt32(data[1]);
            k = Convert.ToInt32(data[2]);
            int expandedVertices = vertices * (k + 1);

            //развертка для графа (вершин в 2 раза больше, чем в исходном графе)
            //первые vertices вершин соответствуют исходному графу
            //вторые vertices вершин - дубликат vertices первых вершин
            //представляется в виде списка смежностей, для каждой вершины v graph(v) содержит
            //список ребер, исходящих из этой вершины, то есть список пар, 
            //где первый элемент пары — вершина, в которую ведёт ребро, а второй элемент — вероятность перехода.
            graph = new List<Edge<double, char>>[expandedVertices];
            for (int i = 0; i < expandedVertices; ++i)
                graph[i] = new List<Edge<double, char>>();


            // в цикле считываем 4 значения. из файла уже считали три первых
            // и вне цикла считаем 2 последних
            for (int i = 3; i < data.Length - 2; i += 4)
            {
                //считываем дуги в формате: 
                //исходящая вершина, входящая вершина, вес дуги и тип дуги (нейтральная, положительная или барьерная)
                int vFrom = Convert.ToInt32(data[i]) - 1;
                int vTo = Convert.ToInt32(data[i + 1]) - 1;
                double probability = Convert.ToDouble(data[i + 2]);
                char edgeType = Convert.ToChar(data[i + 3][0]);

                switch (edgeType)
                {
                    case 'N':
                        for (int j = 0; j <= k; ++j)
                        {
                            graph[vFrom + j * vertices].Add(new Edge<double, char>(vTo + j * vertices, probability, edgeType));
                        }
                        break;
                    case '+':
                        for (int j = 0; j < k; ++j)
                        {
                            graph[vFrom + j * vertices].Add(new Edge<double, char>(vTo + (j + 1) * vertices, probability, edgeType));
                        }

                        graph[vFrom + vertices * k].Add(new Edge<double, char>(vTo + vertices * k, probability, edgeType));

                        break;
                    case 'b':
                        graph[vFrom + vertices * k].Add(new Edge<double, char>(vTo + vertices * k, probability, edgeType));

                        break;
                    default:
                        throw new Exception("Неверный символ типа дуги");
                    // break;
                }

            }

            //проверяем, что в исходном графе сумма вероятностей не больше 1
            //добавляем застревание на концевых вершинах
            for (int i = 0; i < vertices; ++i)
            {
                if (graph[i].Count == 0)
                {
                    graph[i].Add(new Edge<double, char>(i, 1, 'N'));
                }
                else
                {
                    double p = 0;
                    for (int j = 0; j < graph[i].Count; ++j)
                        p += graph[i][j].edgeParam;
                    if (p - 1 > 1e-5)
                    {
                        throw new Exception("Неверный граф!");
                    }
                    else
                        if (p - 1 < 1e-5)
                            graph[i].Add(new Edge<double, char>(i, 1 - p, 'N'));
                }

            }

            // достаточно провести нормировку только вершин-дубликатов
            for (int i = vertices; i < expandedVertices; ++i)
            {
                if (graph[i].Count == 0)
                {
                    graph[i].Add(new Edge<double, char>(i, 1, 'N'));
                }
                else
                {
                    double p = 0;
                    for (int j = 0; j < graph[i].Count; ++j)
                        p += graph[i][j].edgeParam;

                    for (int j = 0; j < graph[i].Count; ++j)
                        graph[i][j].edgeParam /= p;
                }


            }
            vStart = Convert.ToInt32(data[data.Length - 2]) - 1; // стартовая вершина
            time = Convert.ToInt32(data[data.Length - 1]) - 1; // время

        }

     
        //ventil
        private void ComputeMinDistanceForVentil(string[] data)
        {
            int vertices, edges, k;
            List<Edge<int, int>>[] graph;
            int s, f;

            ReadGraphMinDistanceForVentil(data, out vertices, out edges, out k, out graph, out s, out f);
            
            int[] dist;
            Ancestor<int>[] prev;
            DoDejkstraAlgo<int>(graph, (k + 1) * vertices, s, out dist, out prev);

            string[] res = InterpreteMinDistance<int>(vertices, k, s, f, dist, prev);

            PrintResult(res);
        }

        private void ReadGraphMinDistanceForVentil(string[] data, out int vertices, out int edges, out int k, out  List<Edge<int, int>>[] graph, out int vStart, out int vFinish)
        {
            //vertices - количество вершин, edges- количество дуг, k - порядок вентильного пути
            vertices             = Convert.ToInt32(data[0]);
            edges                = Convert.ToInt32(data[1]);
            k                    = Convert.ToInt32(data[2]);
            int expandedVertices = vertices * (k + 1);

            //развертка для графа (вершин в k + 1 раз больше, чем в исходном графе)
            //первые vertices вершин соответствуют исходному графу
            //последующие vertices * k вершин - дубликаты vertices первых вершин
            //представляется в виде списка смежностей, для каждой вершины v graph(v) содержит
            //список ребер, исходящих из этой вершины, то есть список пар, 
            //где первый элемент пары — вершина, в которую ведёт ребро, а второй элемент — вес ребра.
            graph = new List<Edge<int, int>>[expandedVertices];
            for (int i = 0; i < expandedVertices; ++i)
                graph[i] = new List<Edge<int, int>>();


            // в цикле считываем 4 значения. из файла уже считали три первых
            // и вне цикла считаем 2 последних
            for (int i = 3; i < data.Length - 2; i += 4)
            {
                //считываем дуги в формате: 
                //исходящая вершина, входящая вершина, вес дуги и тип дуги (номер множества)
                int vFrom = Convert.ToInt32(data[i]) - 1;
                int vTo = Convert.ToInt32(data[i + 1]) - 1;
                int weight = Convert.ToInt32(data[i + 2]);
                int edgeType = Convert.ToInt32(data[i + 3]);

                if (edgeType < 0 || edgeType > k)
                    throw new Exception("Неверный номер множества дуги");

                for (int j = edgeType + 1; j <= k; ++j)
                {
                    graph[vFrom + j * vertices].Add(new Edge<int, int>(vTo + j * vertices, weight, edgeType));
                }

                if (edgeType == k)
                {
                    graph[vFrom + k * vertices].Add(new Edge<int, int>(vTo + k * vertices, weight, edgeType));
                }
                else
                {
                    graph[vFrom + edgeType * vertices].Add(new Edge<int, int>(vTo + (edgeType + 1) * vertices, weight, edgeType));
                }
            }

            vStart = Convert.ToInt32(data[data.Length - 2]) - 1; // стартовая вершина
            vFinish = Convert.ToInt32(data[data.Length - 1]) - 1; // финишная вершина

        }
        
        private void ComputeRandomWalkForVentil(string[] data)
        {
            //формат ввода
            //количество дуг, количество вершин
            //граф
            //вершина старта, время
            List<Edge<double, int>>[] graph;
            int vertices, edges, k, vStart, time;

            ReadGraphRandomWalkForVentil(data, out vertices, out edges, out k, out graph, out vStart, out time);

            List<double> prob = DoRandomWalkAlgo(graph, (k + 1) * vertices, vStart, time);

            string[] res = InterpreteRandomWalk(vertices, k, prob.ToArray());

            PrintResult(res);
        }

        private void ReadGraphRandomWalkForVentil(string[] data, out int vertices, out int edges, out int k, out List<Edge<double, int>>[] graph, out int vStart, out int time)
        {
            //vertices - количество вершин, edges- количество дуг
            vertices = Convert.ToInt32(data[0]);
            edges = Convert.ToInt32(data[1]);
            k = Convert.ToInt32(data[2]);
            int expandedVertices = (k + 1) * vertices;

            //развертка для графа (вершин в k раза больше, чем в исходном графе)
            //первые vertices вершин соответствуют исходному графу
            //вторые vertices вершин - дубликат vertices первых вершин
            //представляется в виде списка смежностей, для каждой вершины v graph(v) содержит
            //список ребер, исходящих из этой вершины, то есть список пар, 
            //где первый элемент пары — вершина, в которую ведёт ребро, а второй элемент — вероятность перехода.
            graph = new List<Edge<double, int>>[expandedVertices];
            for (int i = 0; i < expandedVertices; ++i)
                graph[i] = new List<Edge<double, int>>();


            // в цикле считываем 4 значения. из файла уже считали три первых
            // и вне цикла считаем 2 последних
            for (int i = 3; i < data.Length - 2; i += 4)
            {
                //считываем дуги в формате: 
                //исходящая вершина, входящая вершина, вес дуги и тип дуги
                int vFrom = Convert.ToInt32(data[i]) - 1;
                int vTo = Convert.ToInt32(data[i + 1]) - 1;
                double probability = Convert.ToDouble(data[i + 2]);
                int edgeType = Convert.ToInt32(data[i + 3]);
                
                if (edgeType < 0 || edgeType > k)
                    throw new Exception("Неверный номер множества дуги");

                for (int j = edgeType + 1; j <= k; ++j)
                {
                    graph[vFrom + j * vertices].Add(new Edge<double, int>(vTo + j * vertices, probability, edgeType));
                }

                if (edgeType == k)
                {
                    graph[vFrom + k * vertices].Add(new Edge<double, int>(vTo + k * vertices, probability, edgeType));
                }
                else
                {
                    graph[vFrom + edgeType * vertices].Add(new Edge<double, int>(vTo + (edgeType + 1) * vertices, probability, edgeType));
                }

            }

            //проверяем, что в исходном графе сумма вероятностей не больше 1
            //добавляем застревание на концевых вершинах
            for (int i = 0; i < vertices; ++i)
            {
                if (graph[i].Count == 0)
                {
                    graph[i].Add(new Edge<double, int>(i, 1, 0));
                }
                else
                {
                    double p = 0;
                    for (int j = 0; j < graph[i].Count; ++j)
                        p += graph[i][j].edgeParam;
                    if (p - 1 > 1e-5)
                    {
                        throw new Exception("Неверный граф!");
                    }
                    else
                        if (p - 1 < 1e-5)
                            graph[i].Add(new Edge<double, int>(i, 1 - p, 0));
                }

            }

            // достаточно провести нормировку только вершин-дубликатов
            for (int i = vertices; i < expandedVertices; ++i)
            {
                if (graph[i].Count == 0)
                {
                    graph[i].Add(new Edge<double, int>(i, 1, 0));
                }
                else
                {
                    double p = 0;
                    for (int j = 0; j < graph[i].Count; ++j)
                        p += graph[i][j].edgeParam;

                    for (int j = 0; j < graph[i].Count; ++j)
                        graph[i][j].edgeParam /= p;
                }


            }
            vStart = Convert.ToInt32(data[data.Length - 2]) - 1; // стартовая вершина
            time = Convert.ToInt32(data[data.Length - 1]) - 1; // время

        }


        //gate
        private void ComputeMinDistanceForGate(string[] data)
        {
            int vertices, edges, k;
            List<Edge<int, int>>[] graph;
            int vStart, vFinish;

            ReadGraphMinDistanceForGate(data, out vertices, out edges, out k, out graph, out vStart, out vFinish);

            int[] dist;
            Ancestor<int>[] prev;
            DoDejkstraAlgo<int>(graph, (k + 1) * vertices, vStart, out dist, out prev);

            string[] res = InterpreteMinDistance<int>(vertices, k, vStart, vFinish, dist, prev);

            PrintResult(res);
        }

        private void ReadGraphMinDistanceForGate(string[] data, out int vertices, out int edges, out int k, out  List<Edge<int, int>>[] graph, out int vStart, out int vFinish)
        {
            //vertices - количество вершин, edges- количество дуг, k - количество шлюзов
            vertices             = Convert.ToInt32(data[0]);
            edges                = Convert.ToInt32(data[1]);
            k                    = Convert.ToInt32(data[2]);
            int expandedVertices = vertices * (k + 1);

            //развертка для графа (вершин в k + 1 раз больше, чем в исходном графе)
            //первые vertices вершин соответствуют исходному графу
            //последующие vertices * k вершин - дубликаты vertices первых вершин
            //представляется в виде списка смежностей, для каждой вершины v graph(v) содержит
            //список ребер, исходящих из этой вершины, то есть список пар, 
            //где первый элемент пары — вершина, в которую ведёт ребро, а второй элемент — вес ребра.
            graph = new List<Edge<int, int>>[expandedVertices];
            for (int i = 0; i < expandedVertices; ++i)
                graph[i] = new List<Edge<int, int>>();


            // в цикле считываем 4 значения. из файла уже считали три первых
            // и вне цикла считаем 2 последних
            for (int i = 3; i < data.Length - 2; i += 4)
            {
                //считываем дуги в формате: 
                //исходящая вершина, входящая вершина, вес дуги и тип дуги (номер множества)
                int vFrom = Convert.ToInt32(data[i]) - 1;
                int vTo = Convert.ToInt32(data[i + 1]) - 1;
                int weight = Convert.ToInt32(data[i + 2]);
                int edgeType;
                if (data[i + 3][0] == 'N')
                    edgeType = 0;
                else
                    edgeType = Convert.ToInt32(data[i + 3]);

                if (edgeType < 0 || edgeType > k)
                    throw new Exception("Неверный номер шлюза");

                if (edgeType == 0)
                {
                    for (int j = 0; j <= k; ++j)
                        graph[vFrom + edgeType * vertices].Add(new Edge<int, int>(vTo + edgeType * vertices, weight, edgeType));
                }
                else 
                {
                    graph[vFrom + edgeType * vertices].Add(new Edge<int, int>(vTo + (edgeType - 1) * vertices, weight, edgeType));
                    graph[vFrom + (edgeType - 1) * vertices].Add(new Edge<int, int>(vTo + edgeType * vertices, weight, edgeType));
                
                }
                
            }

            vStart = Convert.ToInt32(data[data.Length - 2]) - 1; // стартовая вершина
            vFinish = Convert.ToInt32(data[data.Length - 1]) - 1; // финишная вершина

        }
        
        private void ComputeRandomWalkForGate(string[] data)
        {
            //формат ввода
            //количество дуг, количество вершин
            //граф
            //вершина старта, время
            List<Edge<double, int>>[] graph;
            int vertices, edges, k, vStart, time;

            ReadGraphRandomWalkForGate(data, out vertices, out edges, out k, out graph, out vStart, out time);

            List<double> prob = DoRandomWalkAlgo(graph, (k + 1) * vertices, vStart, time);

            string[] res = InterpreteRandomWalk(vertices, k, prob.ToArray());

            PrintResult(res);
        }

        private void ReadGraphRandomWalkForGate(string[] data, out int vertices, out int edges, out int k, out List<Edge<double, int>>[] graph, out int vStart, out int time)
        {
            //vertices - количество вершин, edges- количество дуг
            vertices             = Convert.ToInt32(data[0]);
            edges                = Convert.ToInt32(data[1]);
            k                    = Convert.ToInt32(data[2]);
            int expandedVertices = (k + 1) * vertices;

            //развертка для графа (вершин в k + 1 раза больше, чем в исходном графе)
            //первые vertices вершин соответствуют исходному графу
            //вторые vertices вершин - дубликат vertices первых вершин
            //представляется в виде списка смежностей, для каждой вершины v graph(v) содержит
            //список ребер, исходящих из этой вершины, то есть список пар, 
            //где первый элемент пары — вершина, в которую ведёт ребро, а второй элемент — вероятность перехода.
            graph = new List<Edge<double, int>>[expandedVertices];
            for (int i = 0; i < expandedVertices; ++i)
                graph[i] = new List<Edge<double, int>>();


            // в цикле считываем 4 значения. из файла уже считали три первых
            // и вне цикла считаем 2 последних
            for (int i = 3; i < data.Length - 2; i += 4)
            {
                //считываем дуги в формате: 
                //исходящая вершина, входящая вершина, вес дуги и тип дуги
                int vFrom = Convert.ToInt32(data[i]) - 1;
                int vTo = Convert.ToInt32(data[i + 1]) - 1;
                double probability = Convert.ToDouble(data[i + 2]);
                int edgeType;
                if (data[i + 3][0] == 'N')
                    edgeType = 0;
                else
                    edgeType = Convert.ToInt32(data[i + 3]);

                if (edgeType < 0 || edgeType > k)
                    throw new Exception("Неверный номер шлюза");

                if (edgeType == 0)
                {
                    for (int j = 0; j <= k; ++j)
                        graph[vFrom + edgeType * vertices].Add(new Edge<double, int>(vTo + edgeType * vertices, probability, edgeType));
                }
                else
                {
                    graph[vFrom + edgeType * vertices].Add(new Edge<double, int>(vTo + (edgeType - 1) * vertices, probability, edgeType));
                    graph[vFrom + (edgeType - 1) * vertices].Add(new Edge<double, int>(vTo + edgeType * vertices, probability, edgeType));

                }
               
            }

            //проверяем, что в исходном графе сумма вероятностей не больше 1
            //добавляем застревание на концевых вершинах
            for (int i = 0; i < vertices; ++i)
            {
                if (graph[i].Count == 0)
                {
                    graph[i].Add(new Edge<double, int>(i, 1, 0));
                }
                else
                {
                    double p = 0;
                    for (int j = 0; j < graph[i].Count; ++j)
                        p += graph[i][j].edgeParam;
                    if (p - 1 > 1e-5)
                    {
                        throw new Exception("Неверный граф!");
                    }
                    else
                        if (p - 1 < 1e-5)
                            graph[i].Add(new Edge<double, int>(i, 1 - p, 0));
                }

            }

            // достаточно провести нормализацию только вершин-дубликатов
            for (int i = vertices; i < expandedVertices; ++i)
            {
                if (graph[i].Count == 0)
                {
                    graph[i].Add(new Edge<double, int>(i, 1, 0));
                }
                else
                {
                    double p = 0;
                    for (int j = 0; j < graph[i].Count; ++j)
                        p += graph[i][j].edgeParam;

                    for (int j = 0; j < graph[i].Count; ++j)
                        graph[i][j].edgeParam /= p;
                }


            }
            vStart = Convert.ToInt32(data[data.Length - 2]) - 1; // стартовая вершина
            time = Convert.ToInt32(data[data.Length - 1]) - 1; // время

        }


        //auxilary functions
        private void DoDejkstraAlgo<TypeEdgeType>(List<Edge<int, TypeEdgeType>>[] graph, int vertices, int vStart, out int[] distance, out Ancestor<TypeEdgeType>[] previous)
        {
            distance = new int[vertices];
            for (int i = 0; i < vertices; ++i)
                distance[i] = int.MaxValue;
            distance[vStart] = 0;

            previous = new Ancestor<TypeEdgeType>[vertices];
            for (int i = 0; i < previous.Length; i++)
                previous[i] = new Ancestor<TypeEdgeType>();

            bool[] used = new bool[vertices];
            
            for (int i = 0; i < vertices; ++i)
            {
                int nextVertex = -1;
                for (int j = 0; j < vertices; ++j)
                    if (!used[j] && (nextVertex == -1 || distance[j] < distance[nextVertex]))
                        nextVertex = j;
                if (distance[nextVertex] == int.MaxValue)
                    break;
                used[nextVertex] = true;

                for (int j = 0; j < graph[nextVertex].Count; ++j)
                {
                    int to = graph[nextVertex][j].destVertex,
                        len = graph[nextVertex][j].edgeParam;
                    if (distance[nextVertex] + len < distance[to])
                    {
                        distance[to] = distance[nextVertex] + len;
                        previous[to].prevVertex = nextVertex;
                        previous[to].typeEdge = graph[nextVertex][j].typeEdge;
                    }
                }
            }
        }

        private string[] InterpreteMinDistance<TypeEdgeType>(int vertices, int JumpLevel, int vStart, int vFinish, int[] dist, Ancestor<TypeEdgeType>[] prev)
        {
            int shortestPathLength = int.MaxValue;
            List<Ancestor<TypeEdgeType>> path = new List<Ancestor<TypeEdgeType>>();
            int pathFinish = 0;

            //определяем длину минимального пути shortpath
            //и вершину, в которой он оканчивается finish
            for (int i = 0; i <= JumpLevel; ++i)
                if (dist[vFinish + i * vertices] < shortestPathLength)
                {
                    shortestPathLength = dist[vFinish + i * vertices];
                    pathFinish = vFinish + i * vertices;
                }

            //находим минимальный путь path
            for (; pathFinish != vStart; pathFinish = prev[pathFinish].prevVertex)
                path.Add(new Ancestor<TypeEdgeType>(pathFinish, prev[pathFinish].typeEdge));
            path.Add(new Ancestor<TypeEdgeType>(vStart, prev[vStart].typeEdge));   // typeEdge может быть любым - он не используется у стартовой вершины
            path.Reverse(0, path.Count);

            string[] outArray = new string[path.Count];
            for (int i = 1; i < path.Count; ++i)
            {
                outArray[i - 1] = (path[i - 1].prevVertex % vertices + 1) + " " + (path[i].prevVertex % vertices + 1) + " " + (path[i].typeEdge);
            }

            outArray[outArray.Length - 1] = "len = " + shortestPathLength;

            return outArray;
        }

        private List<double> DoRandomWalkAlgo<TypeEdgeType>(List<Edge<double, TypeEdgeType>>[] graph, int vertices, int s, int t)
        {
            List<double> currentState = new List<double>(vertices);
            List<double> nextState = new List<double>(vertices);
            for (int i = 0; i < vertices; ++i)
            {
                currentState.Add(0);
                nextState.Add(0);
            }
            currentState[s] = 1;

            for (int i = 0; i < t; ++i)
            {
                for (int j = 0; j < vertices; ++j)
                    if (currentState[j] != 0)
                        for (int k = 0; k < graph[j].Count; ++k)
                            nextState[graph[j][k].destVertex] += currentState[j] * graph[j][k].edgeParam;
                for (int k = 0; k < vertices; ++k)
                {
                    currentState[k] = nextState[k];
                    nextState[k] = 0;
                }
            }
            return currentState;

        }

        private string[] InterpreteRandomWalk(int vertices, int k, double[] prob)
        {
            double[] res = new double[vertices];
            for (int i = 0; i < vertices; ++i)
            {
                res[i] = 0;
                for (int j = 0; j <= k; ++j)
                    res[i] += prob[i + vertices * j];
            }
            return Array.ConvertAll(res, new Converter<double, string>(x => x.ToString()));
        }

        private void PrintResult(string[] output)
        {
            
            if (!outFile.EndsWith(".txt"))
            {
                if (File.Exists(inFile))
                {
                    outFile = Path.GetDirectoryName(inFile) + "\\" + 
                         Path.GetFileNameWithoutExtension(inFile) + "_output" + Path.GetExtension(inFile);
                }
                else
                {
                    outFile = Path.GetDirectoryName(inFile) + "\\" + "output.txt";
                }

                MessageBox.Show("Результат будет сохранен в файл " + outFile, "Файл не выбран", MessageBoxButtons.OK);                
            }

            File.WriteAllLines(outFile, output);
        }        

        private void inputFileButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(inFile))
            {
                openFileDialog1.FileName = Path.GetFileName(inFile);
            }
            else 
            {
                openFileDialog1.FileName = "";
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                inFile = openFileDialog1.FileName;

            outFile = "";
        }
        
        private void outputFileButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(inFile))
            {
                saveFileDialog1.FileName = //Path.GetDirectoryName(inFile) + "\\" + 
                     Path.GetFileNameWithoutExtension(inFile) + "_output" + Path.GetExtension(inFile);
            }
            else
            {
                saveFileDialog1.FileName = "output.txt";
            }
            
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                outFile = saveFileDialog1.FileName;
        }

        private string inFile = "";
        private string outFile = "";
    } 
 
   public class Edge <EdgeParamType, TypeEdgeType>
    {
        public int destVertex;
        public EdgeParamType edgeParam;
        public TypeEdgeType typeEdge;
        public Edge(int x, EdgeParamType y, TypeEdgeType z)
       {
           destVertex = x;
           edgeParam = y;
           typeEdge = z;
       }
    }

   public class Ancestor <TypeEdgeType>
    {
        public int prevVertex;
        public TypeEdgeType typeEdge;
        public Ancestor(int x, TypeEdgeType y)
        {
            prevVertex = x;
            typeEdge = y;
        }

        public Ancestor() { }
    }
}