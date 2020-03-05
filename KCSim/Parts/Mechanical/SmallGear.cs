using System;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical
{
    public class SmallGear : Gear
    {
        public SmallGear(string name = "") : base(name)
        {
        }

        public override uint GetNumTeeth()
        {
            return 14;
        }
    }
}
