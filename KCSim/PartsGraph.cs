using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim
{
    public class PartsGraph : IPartsGraph
    {
        private readonly Dictionary<Torqueable, ISet<Coupling>> graph;

        public PartsGraph()
        {
            graph = new Dictionary<Torqueable, ISet<Coupling>>();
        }

        public void AddVerticesAndEdge(Coupling coupling)
        {
            if (!graph.ContainsKey(coupling.Input))
            {
                graph[coupling.Input] = new HashSet<Coupling>();
            }
            graph[coupling.Input].Add(coupling);

            if (!graph.ContainsKey(coupling.Output))
            {
                graph[coupling.Output] = new HashSet<Coupling>();
            }
            graph[coupling.Output].Add(coupling);
        }

        public ISet<Coupling> GetCouplings(Torqueable node)
        {
            ISet<Coupling> couplings = graph.GetValueOrDefault(node, ImmutableHashSet<Coupling>.Empty);
            return new HashSet<Coupling>(couplings);
        }

        public void RemoveEdge(Coupling coupling)
        {
            graph[coupling.Input].Remove(coupling);
            graph[coupling.Output].Remove(coupling);
        }
    }
}
