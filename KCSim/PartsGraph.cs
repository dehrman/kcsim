using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using ConcurrentCollections;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim
{
    public class PartsGraph : IPartsGraph
    {
        private readonly ConcurrentDictionary<Torqueable, ConcurrentHashSet<Coupling>> graph;
        private readonly ConcurrentHashSet<Coupling> registeredCouplings;

        public PartsGraph()
        {
            graph = new ConcurrentDictionary<Torqueable, ConcurrentHashSet<Coupling>>();
            registeredCouplings = new ConcurrentHashSet<Coupling>();
        }

        public void AddVerticesAndEdge(Coupling coupling)
        {
            if (!graph.ContainsKey(coupling.Input))
            {
                graph.TryAdd(coupling.Input, new ConcurrentHashSet<Coupling>());
            }
            graph[coupling.Input].Add(coupling);

            if (!graph.ContainsKey(coupling.Output))
            {
                graph.TryAdd(coupling.Output, new ConcurrentHashSet<Coupling>());
            }
            graph[coupling.Output].Add(coupling);

            registeredCouplings.Add(coupling);
        }

        public void RemoveEdge(Coupling coupling)
        {
            if (graph.ContainsKey(coupling.Input))
            {
                graph[coupling.Input].TryRemove(coupling);
            }
            if (graph.ContainsKey(coupling.Output))
            {
                graph[coupling.Output].TryRemove(coupling);
            }
            registeredCouplings.TryRemove(coupling);
        }

        public ISet<Coupling> GetCouplings(Torqueable node)
        {
            ConcurrentHashSet<Coupling> couplings;
            if (!graph.TryGetValue(node, out couplings))
            {
                couplings = new ConcurrentHashSet<Coupling>();
            }
            return new HashSet<Coupling>(couplings);
        }

        public bool IsEdgeStillPresent(Coupling coupling)
        {
            return registeredCouplings.Contains(coupling);
        }

        public IEnumerable<KeyValuePair<Torqueable, Coupling>> GetLeafVertices()
        {
            return graph
                .Where(kvp => kvp.Value.Count == 1)
                .Select(kvp => new KeyValuePair<Torqueable, Coupling>(kvp.Key, kvp.Value.ElementAt(0)));
        }
    }
}
