using AvaloniaGraphControl;
using ModelA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper
{
    public static class GraphHelper
    {
        public static Graph ToGraph(this Person person)
        {
            var graph = new Graph();

            if (person.Father != null && person.Mother != null)
            {
                //TODO: see how position correctly the name inside the shape

                //TODO:use zoom control https://github.com/wieslawsoltes/PanAndZoom?tab=readme-ov-file

                var edge = new Edge(person.Father, person.Name, Edge.Symbol.None, Edge.Symbol.None);

                graph.Edges.Add(edge);
            }

            return graph;
        }
    }
}
