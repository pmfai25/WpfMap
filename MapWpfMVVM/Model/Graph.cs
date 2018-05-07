using MapWpfMVVM.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapWpfMVVM
{
    public class Graph:IGraph
    {
        private int numberOfTop;
        private List<TopGraph> topGraphs;

        public int NumberOfTop { get => numberOfTop; set => numberOfTop = value; }
        public List<TopGraph> TopGraphs { get
            {
                if (topGraphs==null)
                {
                    topGraphs = new List<TopGraph>();
                }
                return topGraphs;
            } set => topGraphs = value; }


        public bool ConvertTextToList()
        {
            try
            {
                topGraphs = ConvertTextToList(@"C:\Users\Admin\source\repos\MapWpf\MapWpfMVVM\Data\Data.txt");
                numberOfTop = topGraphs.Count;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private List<TopGraph> ConvertTextToList(string path)
        {
            try
            {
                var r = new bool[10];
                string line;
                List<TopGraph> list = new List<TopGraph>();
                using (StreamReader file = new StreamReader(path))
                {
                    int count = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        TopGraph topGraph = new TopGraph();
                        topGraph.Id = count;
                        var data = line.Split(' ');
                        var num = int.Parse(data[0]);
                        for (int i = 1; i <= num * 2; i++)
                        {
                            if (i + 1 <= num * 2)
                            {
                                topGraph.PointNeighbour.Add(int.Parse(data[i]), double.Parse(data[i + 1]));
                                //topGraph.PointNeighbour.Add(item);
                            }
                            i++;
                        }
                        StringBuilder builder = new StringBuilder();
                        for (int j = num * 2 + 1; j < data.Length - 2; j++)
                        {
                            builder.Append(data[j]);
                            builder.Append(" ");
                        }
                        topGraph.NumOfNeighbour = topGraph.PointNeighbour.Count;
                        topGraph.Name = builder.ToString().Trim();
                        topGraph.GetPoints = new Point(double.Parse(data[data.Length - 2]), double.Parse(data[data.Length - 1]));
                        count++;
                        list.Add(topGraph);
                    }
                    file.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Tuple<double,int[]> Disjkstra(int start,int end)
        {
            var distance = new double[NumberOfTop + 1];
            
            var parent = new int[NumberOfTop];
            parent.AsParallel().ForAll(x => x = start);
            var visit = new bool[NumberOfTop];
            Hashtable hashtable = TopGraphs[start].PointNeighbour;
            Parallel.For(0, distance.Length, x => 
            {
                distance[x]= 2147483647;
            });
            Parallel.For(0,NumberOfTop, x =>
            {
                parent[x] = start;
            });

            Parallel.ForEach(hashtable.Keys.OfType<int>(), x =>
            {                
                distance[x] =(double) hashtable[x];
            });
            TopGraph[] graphsArray = TopGraphs.ToArray();
            visit[start] = true;
            while (true)
            {
                var min = NumberOfTop;
                for (int i = NumberOfTop - 1; i >= 0; --i)
                {
                    if (visit[i] == false && distance[i] < distance[min])
                    {
                        min = i;                        
                    }
                }
                if (min == NumberOfTop)
                    break;
                if (min == end)
                    break;
                var v = min;
                visit[v] = true;
                hashtable = TopGraphs[v].PointNeighbour;
                foreach (int u in TopGraphs[v].PointNeighbour.Keys)
                {
                    var sum = distance[v] +(double) TopGraphs[v].PointNeighbour[u];
                    if (visit[u] == false && distance[u] > sum)
                    {
                        distance[u] = sum;                        
                        parent[u] = v;
                    }
                }
            }
            List<int> result = new List<int>();
            int temp = end;
            while (temp != start)
            {
                result.Add(temp);
                temp = parent[temp];
            }
            result.Add(temp);
            result.Reverse();

            return new Tuple<double, int[]>( distance[end],result.ToArray());
        }
       
    }
}
