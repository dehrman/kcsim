using System;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical
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
