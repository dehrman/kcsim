using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Logical;
using KCSim.Physics;
using static KCSim.Parts.Mechanical.Atomic.Axle;

namespace KCSim
{
    public class GateMonitor : IGateMonitor
    {
        public T RegisterGate<T>(T gate) where T : Gate
        {
            gate.InputA.OnForceChange = GetOnForceChangeDelegate(gate, gate.InputA);
            gate.InputB.OnForceChange = GetOnForceChangeDelegate(gate, gate.InputB);
            gate.Power.OnForceChange = GetOnForceChangeDelegate(gate, gate.Power);
            gate.Output.OnForceChange = GetOnForceChangeDelegate(gate, gate.Output);
            gate.OutputInverse.OnForceChange = GetOnForceChangeDelegate(gate, gate.OutputInverse);

            return gate;
        }

        private OnForceChangeDelegate GetOnForceChangeDelegate(Gate gate, Torqueable torqueable)
        {
            return (oldForce, newForce) =>
                System.Diagnostics.Debug.WriteLine(gate + ", " + torqueable + " changed from " + oldForce + " to " + newForce);
        }
    }
}
