using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.State
{
    public class StatefulGate : Gate
    {
        public readonly Axle Q; // aliased to Output

        public StatefulGate(string name = "") : base(name)
        {
            Q = new Axle(name + " Q");
        }

        public override bool RequiresPower()
        {
            return true;
        }
    }
}
