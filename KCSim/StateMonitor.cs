using System.Collections.Generic;
using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.State;
using KCSim.Physics;
using static KCSim.Physics.Torqueable;

namespace KCSim
{
    public class StateMonitor : IStateMonitor
    {
        private readonly ICouplingMonitor couplingMonitor;

        public StateMonitor(
            ICouplingMonitor couplingMonitor)
        {
            this.couplingMonitor = couplingMonitor;
        }

        public T RegisterGate<T>(T gate) where T : Gate
        {
            ISet<Torqueable> coupledTorqueables = gate.GetType().GetFields()
                .Where(field => typeof(Torqueable).IsAssignableFrom(field.FieldType))
                .Select(field => field.GetValue(gate))
                .Cast<Torqueable>()
                .Where(torqueable => couplingMonitor.IsCoupled(torqueable))
                .ToHashSet();

            foreach (var torqueable in coupledTorqueables)
            {
                torqueable.OnForceChange += GetOnForceChangeDelegate(gate, torqueable);
            }

            return gate;
        }

        private OnForceChangeDelegate GetOnForceChangeDelegate(Gate gate, Torqueable torqueable)
        {
            return (oldForce, newForce) =>
                System.Diagnostics.Debug.WriteLine(gate + ", " + torqueable + " changed from " + oldForce + " to " + newForce);
        }
    }
}
