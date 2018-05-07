using System;
using System.Collections.Generic;

namespace MapWpfMVVM.Model
{
    public interface IGraph
    {
        int NumberOfTop { get ; set ; }
        List<TopGraph> TopGraphs { get ; set ; }
        bool ConvertTextToList();
        Tuple<double, int[]> Disjkstra(int start, int end);
    }
}
