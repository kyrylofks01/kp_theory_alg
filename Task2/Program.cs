using System.Diagnostics;

namespace Task2
{
    internal class Program
    {
        static int V = 11;
        static string[] placeNames = {
            "Червоний Університет",       // 0
            "Андріївська церква",         // 1
            "Михайлівський собор",        // 2
            "Золоті Ворота",              // 3
            "Лядські ворота",             // 4
            "Фунікулер",                  // 5
            "Київська політехніка",       // 6
            "Фонтан на Хрещатику",        // 7
            "Софія Київська",             // 8
            "Національна філармонія",     // 9
            "Музей однієї вулиці"         // 10
        };

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            int[,] graph = new int[V, V]; // матриця суміжності

            // двосторонні дороги
            // 7 <-> 1 (КПІ <-> Червоний Університет)
            graph[6, 0] = 1200; graph[0, 6] = 1200;
            // 1 <-> 4 (Червоний Університет <-> Золоті Ворота)
            graph[0, 3] = 1200; graph[3, 0] = 1200;
            // 4 <-> 9 (Золоті Ворота <-> Софія Київська)
            graph[3, 8] = 800; graph[8, 3] = 800;
            // 9 <-> 2 (Софія Київська <-> Андріївська церква)
            graph[8, 1] = 800; graph[1, 8] = 800;
            // 2 <-> 11 (Андріївська церква <-> Музей однієї вулиці)
            graph[1, 10] = 800; graph[10, 1] = 800;
            // 8 <-> 5 (Фонтан <-> Лядські ворота)
            graph[7, 4] = 1600; graph[4, 7] = 1600;
            // 5 <-> 10 (Лядські ворота <-> Національна філармонія)
            graph[4, 9] = 1200; graph[9, 4] = 1200;
            // 8 <-> 10 (Фонтан <-> Національна філармонія)
            graph[7, 9] = 800; graph[9, 7] = 800;
            // 3 <-> 6 (Михайлівський собор <-> Фунікулер)
            graph[2, 5] = 400; graph[5, 2] = 400;

            // односторонні
            graph[1, 2] = 400;  // 2 -> 3
            graph[10, 5] = 400; // 11 -> 6
            graph[2, 8] = 800;  // 3 -> 9
            graph[8, 4] = 800;  // 9 -> 5
            graph[4, 2] = 800;  // 5 -> 3
            graph[3, 7] = 800;  // 4 -> 8

            int start = 6; // КПІ (точка 7, індекс 6)
            int end = 10;  // Музей однієї вулиці (точка 11, індекс 10)

            //алгоритм Форда-Фалкерсона (на основі DFS)
            int[,] graphForFF = (int[,])graph.Clone();
            Stopwatch swFF = Stopwatch.StartNew();
            int maxFlowFF = GetMaxFlow(graphForFF, start, end, useBFS: false);
            swFF.Stop();

            Console.WriteLine($"[Алгоритм Форда-Фалкерсона (DFS)]");
            Console.WriteLine($"Максимальна кількість авто: {maxFlowFF}");
            Console.WriteLine($"Час виконання: {swFF.Elapsed.TotalMilliseconds} мс\n");

            // алгоритм Едмондса-Карпа (на основі BFS)
            int[,] graphForEK = (int[,])graph.Clone();
            Stopwatch swEK = Stopwatch.StartNew();
            int maxFlowEK = GetMaxFlow(graphForEK, start, end, useBFS: true);
            swEK.Stop();

            Console.WriteLine($"[Алгоритм Едмондса-Карпа (BFS)]");
            Console.WriteLine($"Максимальна кількість авто: {maxFlowEK}");
            Console.WriteLine($"Час виконання: {swEK.Elapsed.TotalMilliseconds} мс\n");

            Stopwatch swTarjan = Stopwatch.StartNew();
            FindSCCsTarjan(graph);
            swTarjan.Stop();
            Console.WriteLine($"\n[Час виконання алгоритму Тар'яна: {swTarjan.Elapsed.TotalMilliseconds:F4} мс]\n");

            Console.ReadLine();
        }
        static int GetMaxFlow(int[,] residualGraph, int source, int sink, bool useBFS)
        {
            int[] parent = new int[V];
            int maxFlow = 0;

            while (useBFS ? BFS(residualGraph, source, sink, parent) : DFS(residualGraph, source, sink, parent))
            {
                int pathFlow = int.MaxValue;
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    pathFlow = Math.Min(pathFlow, residualGraph[u, v]);
                }

                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    residualGraph[u, v] -= pathFlow;
                    residualGraph[v, u] += pathFlow;
                }

                maxFlow += pathFlow;
            }

            return maxFlow;
        }

        static bool BFS(int[,] residualGraph, int source, int sink, int[] parent)
        {
            bool[] visited = new bool[V];
            Queue<int> queue = new Queue<int>();

            queue.Enqueue(source);
            visited[source] = true;
            parent[source] = -1;

            while (queue.Count > 0)
            {
                int u = queue.Dequeue();

                for (int v = 0; v < V; v++)
                {
                    if (!visited[v] && residualGraph[u, v] > 0)
                    {
                        if (v == sink)
                        {
                            parent[v] = u;
                            return true;
                        }
                        queue.Enqueue(v);
                        parent[v] = u;
                        visited[v] = true;
                    }
                }
            }
            return false;
        }

        static bool DFS(int[,] residualGraph, int source, int sink, int[] parent)
        {
            bool[] visited = new bool[V];
            Stack<int> stack = new Stack<int>();

            stack.Push(source);
            visited[source] = true;
            parent[source] = -1;

            while (stack.Count > 0)
            {
                int u = stack.Pop();

                for (int v = 0; v < V; v++)
                {
                    if (!visited[v] && residualGraph[u, v] > 0)
                    {
                        parent[v] = u;
                        visited[v] = true;

                        if (v == sink)
                            return true;

                        stack.Push(v);
                    }
                }
            }
            return false;
        }

        static int time = 0;
        static int sccCount = 0;

        static void FindSCCsTarjan(int[,] graph)
        {
            int[] ids = new int[V];
            int[] low = new int[V];
            bool[] onStack = new bool[V];
            Stack<int> stack = new Stack<int>();

            // Ініціалізація (-1 означає, що вершину ще не відвідували)
            for (int i = 0; i < V; i++)
            {
                ids[i] = -1;
                low[i] = -1;
            }

            time = 0;
            sccCount = 0;

            Console.WriteLine("[Компоненти сильної зв'язності (Алгоритм Тар'яна)]");

            for (int i = 0; i < V; i++)
                if (ids[i] == -1) // Якщо вершина не відвідана, запускаємо DFS
                    TarjanDFS(i, graph, ids, low, onStack, stack);

            Console.WriteLine("\n[ЗВ'ЯЗНІСТЬ]");
            Console.WriteLine($"Граф розбивається на {sccCount} компонент{(sccCount > 1 ? "и" : "")} сильної зв'язності.");
            if (sccCount == 1)
                Console.WriteLine("Граф є СИЛЬНО-ЗВ'ЯЗНИМ (з будь-якої точки можна дістатися до будь-якої іншої).");
            else
            {
                Console.WriteLine("Граф є СЛАБКО-ЗВ'ЯЗНИМ. Через наявність односторонніх вулиць, ");
                Console.WriteLine("існують маршрути, з яких неможливо повернутися назад (утворюються цикли або тупики).");
            }
        }

        static void TarjanDFS(int u, int[,] graph, int[] ids, int[] low, bool[] onStack, Stack<int> stack)
        {
            // Присвоюємо вершині час виявлення та попереднє значення "найнижчої" вершини
            ids[u] = low[u] = ++time;
            stack.Push(u);
            onStack[u] = true;

            for (int v = 0; v < V; v++)
            {
                if (graph[u, v] > 0) // Якщо є дорога з u у v
                {
                    if (ids[v] == -1) // Якщо сусід ще не відвіданий
                    {
                        TarjanDFS(v, graph, ids, low, onStack, stack);
                        low[u] = Math.Min(low[u], low[v]);
                    }
                    else if (onStack[v]) // Якщо сусід вже відвіданий і знаходиться в поточному шляху (стеку)
                    {
                        // Знайшли зворотне ребро (цикл)
                        low[u] = Math.Min(low[u], ids[v]);
                    }
                }
            }

            if (ids[u] == low[u])
            {
                sccCount++;
                Console.WriteLine($"\nКомпонента {sccCount}:");

                int node;
                do
                {
                    node = stack.Pop();
                    onStack[node] = false;
                    Console.WriteLine($" - {placeNames[node]}");
                } while (node != u);
            }
        }
    }
}
