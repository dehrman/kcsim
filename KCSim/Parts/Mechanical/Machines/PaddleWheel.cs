using System;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim.Parts.Mechanical
{
    public class PaddleWheel : Gear
    {
        public PaddleWheel(string name = "") : base(name)
        {
        }

        public override bool UpdateForce(Torqueable source, Force force)
        {
            return base.UpdateForce(source, force);
        }

        public override uint GetNumTeeth()
        {
            return 4;
        }
    }
}
