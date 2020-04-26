using System;
using System.Linq;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.LogicalBlocks
{
    public class LogicalBlock
    {
        public readonly Axle[] Inputs;
        public readonly Axle[] Outputs;
        public readonly Axle Output; // mapped to Outputs[0]
        public readonly Axle Power;

        private readonly string name;

        public LogicalBlock(
            int numInputs,
            int numOutputs = 1,
            string name = "logical block")
        {
            this.name = name;

            // Create the inputs.
            Inputs = Enumerable.Range(0, numInputs)
                .Select(i => new Axle(name + "; input " + i))
                .ToArray();

            // Create the outputs.
            Outputs = Enumerable.Range(0, numInputs)
                .Select(i => new Axle(name + "; output " + i))
                .ToArray();

            Output = Outputs[0];

            // Create the power.
            Power = new Axle(name + "; power");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
