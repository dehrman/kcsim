using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim
{
    public interface IPartsGraph
    {
        void AddVerticesAndEdge(Coupling coupling);
        void RemoveEdge(Coupling coupling);

        ISet<Coupling> GetCouplings(Torqueable node);
    }
}
