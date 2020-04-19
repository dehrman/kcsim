using System;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical.Atomic
{
    public class MediumGear : Gear
    {
        public MediumGear(string name = "") : base(name)
        {
        }

        public override uint GetNumTeeth()
        {
            return 34;
        }
    }
}
