using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.State
{
    public class StatefulGate : Gate
    {
        public readonly Axle Set;
        public readonly Axle Reset;
        public readonly Axle Data;
        public readonly Axle Clock;
        public readonly Axle Q;
        public readonly Axle QInverse;

        public StatefulGate(string name = "") : base(name)
        {
            Set = new Axle(name + "; set");
            Reset = new Axle(name + "; reset");
            Data = new Axle(name + "; data");
            Clock = new Axle(name + "; clock");
            Q = Output;
            QInverse = OutputInverse;
        }
    }
}
