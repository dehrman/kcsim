using System.Collections.Generic;
using System.Linq;
using System.Text;
using KCSim.Parts.Logical;
using KCSim.Physics;
using static KCSim.Physics.Torqueable;

namespace KCSim
{
    /// <summary>
    /// A monitor for any <see cref="Gate"/> that has been explicitly registered for monitoring.
    ///
    /// This monitor is used primarily for debugging (e.g. printing debug statements as gates' inputs
    /// and outputs change) and for validating certain conditions about gates.
    /// </summary>
    public class GateMonitor : IGateMonitor
    {
        private readonly IPartsGraph partsGraph;
        private readonly ICouplingMonitor couplingMonitor;

        private readonly IDictionary<Gate, ISet<Torqueable>> gatePower = new Dictionary<Gate, ISet<Torqueable>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GateMonitor"/> class.
        /// </summary>
        /// <param name="partsGraph">The parts graph.</param>
        /// <param name="couplingMonitor">The coupling monitor.</param>
        public GateMonitor(
            IPartsGraph partsGraph,
            ICouplingMonitor couplingMonitor)
        {
            this.partsGraph = partsGraph;
            this.couplingMonitor = couplingMonitor;
        }

        /// <summary>
        /// Register a <see cref="Gate"/> for monitoring.
        /// </summary>
        /// <typeparam name="T">the type of gate</typeparam>
        /// <param name="gate">the gate to register</param>
        /// <returns></returns>
        public T RegisterGate<T>(T gate) where T : Gate
        {
            MonitorPower(gate);
            MonitorForceChanges(gate);
            return gate;
        }

        /// <summary>
        /// Validate that there are no unpowered gates that should have power.
        /// </summary>
        /// <exception cref="UnpoweredGateException">Thrown when there are unpowered gates which should have power.</exception>
        public void ValidateNoUnpoweredGates()
        {
            var unpoweredGates = gatePower
                .Where(kvp => (kvp.Value == null || kvp.Value.Count == 0))
                .Select(kvp => kvp.Key)
                .ToArray();

            if (unpoweredGates.Length == 0)
            {
                return;
            }

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < unpoweredGates.Length; i++)
            {
                stringBuilder.Append("UNPOWERED GATE " + i + ": " + unpoweredGates[i]);
            }

            throw new UnpoweredGateException("Found gates that require powere but which have no connected power. " + stringBuilder.ToString());
        }

        private void MonitorPower<T>(T gate) where T : Gate
        {
            if (!gate.RequiresPower())
            {
                return;
            }

            // Update this monitor any time the gate's power is attached to an input.
            couplingMonitor.OnCoupledToInput(gate.Power, c => AddInputToGatePowerDictionary(gate, c.Input));

            // Get existing couplings where this gate's power is an output.
            var inputCouplings = partsGraph.GetCouplings(gate.Power)
                .Where(c => c.IsOutput(gate.Power))
                .ToList();

            // Update this monitor any time one of those couplings is removed.
            foreach (var coupling in inputCouplings)
            {
                // Add this coupling to this monitor's knowledge of inputs attached to this gate's power.
                gatePower[gate].Add(coupling.Input);
                couplingMonitor.OnCouplingRemoved(coupling, c => RemoveInputFromGatePowerDictionary(gate, c.Input));
            }
        }

        private void AddInputToGatePowerDictionary(Gate gate, Torqueable input)
        {
            if (!gatePower.ContainsKey(gate))
            {
                gatePower[gate] = new HashSet<Torqueable>();
            }
            gatePower[gate].Add(input);
        }

        private void RemoveInputFromGatePowerDictionary(Gate gate, Torqueable input)
        {
            if (!gatePower.ContainsKey(gate))
            {
                return;
            }
            gatePower[gate].Remove(input);
        }

        private void MonitorForceChanges<T>(T gate) where T : Gate
        {
            ISet<Torqueable> coupledTorqueables = gate.GetType().GetFields()
                .Where(field => typeof(Torqueable).IsAssignableFrom(field.FieldType))
                .Select(field => field.GetValue(gate))
                .Cast<Torqueable>()
                .Where(torqueable => torqueable != null)
                .Where(torqueable => couplingMonitor.IsCoupled(torqueable))
                .ToHashSet();

            foreach (var torqueable in coupledTorqueables)
            {
                torqueable.OnForceChange += GetOnForceChangeDelegate(gate, torqueable);
            }

            ISet<Torqueable[]> torqueableArrays = gate.GetType().GetFields()
                .Where(field => typeof(Torqueable[]).IsAssignableFrom(field.FieldType))
                .Select(field => field.GetValue(gate))
                .Where(torqueableArray => torqueableArray != null)
                .Cast<Torqueable[]>()
                .ToHashSet();

            foreach (var torqueableArray in torqueableArrays)
            {
                foreach (var torqueable in torqueableArray)
                {
                    if (couplingMonitor.IsCoupled(torqueable))
                    {
                        torqueable.OnForceChange += GetOnForceChangeDelegate(gate, torqueable);
                    }
                }
            }
        }

        private OnForceChangeDelegate GetOnForceChangeDelegate(Gate gate, Torqueable torqueable)
        {
            return (oldForce, newForce) =>
                System.Diagnostics.Debug.WriteLine(gate + ", " + torqueable + " changed from " + oldForce + " to " + newForce);
        }
    }
}
