using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.State
{
    public class StatefulGate : Gate
    {
        public readonly Axle Q; // aliased to Output
        public readonly Axle QInverse; // aliased to OutputInverse

        public StatefulGate(string name = "") : base(name)
        {
            Q = new Axle(name + " Q");
            QInverse = new Axle(name + " QInverse");
        }

        public override bool RequiresPower()
        {
            return true;
        }
    }
}
