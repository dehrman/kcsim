using System;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim.Parts.Mechanical
{
    public class PaddleWheel : Torqueable
    {
        public PaddleWheel(string name = "") : base(name)
        {
        }

        public override bool UpdateForce(Torqueable source, Force force)
        {
            return base.UpdateForce(source, force);
        }
    }
}
