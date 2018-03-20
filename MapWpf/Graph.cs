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
            return new Tuple<double, int[]>( distance[end],parent);
        }
        /*
        public double Dijkstra(int start, int end)
        {
            var list = new List<int>();
            double[] distance = new double[_vertexNumber + 1];
            var parent = new int[_vertexNumber];
            bool[] visited = new bool[_vertexNumber];
            int i, j, k;
            for (i = 0; i < _vertexNumber; ++i)
            {
                distance[i] = _graphMatrix[start, i];
                parent[i] = start;
                visited[i] = false;
            }
            visited[start] = true;
            distance[start] = MaxWeight;
            distance[_vertexNumber] = MaxWeight;
            while (true)
            {
                var min = _vertexNumber;
                for (i = _vertexNumber - 1; i >= 0; --i)
                    if (visited[i] == false && distance[i] < distance[min])
                        min = i;
                if (min == _vertexNumber)
                    break;
                if (min == end)
                    break;
                var v = min;
                visited[v] = true;
                foreach (var u in _graphList[v])
                {
                    var sum = distance[v] + _graphMatrix[v, u];
                    if (visited[u] == false && distance[u] > sum)
                    {
                        distance[u] = sum;
                        list.Add(v);
                        parent[u] = v;
                    }
                }
            }
            var data = GetVertexPath(start, end, parent);
            return distance[end];
        }
        */
    }
}
