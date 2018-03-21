using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWpf
{
    public class Graph
    {
        private int numberOfTop;
        private List<TopGraph> topGraphs;

        public int NumberOfTop { get => numberOfTop; set => numberOfTop = value; }
        public List<TopGraph> TopGraphs { get => topGraphs; set => topGraphs = value; }

        public Graph()
        {

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
                    var sum = distance[v] +(double) TopGraphs[v].PointNeighbour[u];// _graphMatrix[v, u];
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

            return new Tuple<double, int[]>( distance[end],parent);
        }
       
    }
}
