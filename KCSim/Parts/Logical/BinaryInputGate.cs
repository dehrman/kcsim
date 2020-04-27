using System;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public abstract class BinaryInputGate : Gate
    {
        public readonly Axle InputA;
        public readonly Axle InputB;
        public readonly Axle Output;
        public readonly Axle OutputInverse;

        public BinaryInputGate(string name = "") : base(name)
        {
            // Create the I/O.
            InputA = new Axle(name + " input A");
            InputB = new Axle(name + " input B");
            Output = new Axle(name + " output");
            OutputInverse = new Axle(name + " output inverse");
        }
    }
}
