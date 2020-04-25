using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class MultiInputGate<T> : Gate
        where T : Gate
    {
        public readonly Axle[] Inputs;

        private readonly T[] gates;
        private readonly ICouplingService couplingService;

        public MultiInputGate(
            int numInputs,
            T[] gates,
            ICouplingService couplingService,
            string name = "multi-input gate") : base(name)
        {
            Contract.Requires(numInputs >= 2, "the number of inputs to a multi-input gate must be at least 2");

            this.gates = gates;
            this.couplingService = couplingService;

            // Create the inputs.
            Inputs = Enumerable.Range(0, numInputs)
                .Select(i => new Axle(name + "; input " + i))
                .ToArray();

            ConnectGates();
        }

        private void ConnectGates()
        {
            Queue<Axle> disconnectedAxles = new Queue<Axle>(Inputs);

            foreach (var gate in gates)
            {
                var inputA = disconnectedAxles.Dequeue();
                var inputB = disconnectedAxles.Dequeue();

                couplingService.CreateNewLockedCoupling(inputA, gate.InputA);
                couplingService.CreateNewLockedCoupling(inputB, gate.InputB);

                disconnectedAxles.Enqueue(gate.Output);

                // Connect power—unrelated to the connections between gates, simply placed here for performance reasons.
                couplingService.CreateNewLockedCoupling(Power, gate.Power);
            }

            // Connect the output.
            Contract.Assert(disconnectedAxles.Count == 1);
            var lastAxle = disconnectedAxles.Dequeue();
            couplingService.CreateNewLockedCoupling(lastAxle, Output);
        }

        public override bool RequiresPower()
        {
            return true;
        }
    }
}
