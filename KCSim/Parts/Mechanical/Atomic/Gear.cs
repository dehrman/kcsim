using System;
using System.Collections.Generic;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical.Atomic
{
    public abstract class Gear : Torqueable
    {
        public Gear(string name = "") : base(name)
        {
        }

        public abstract uint GetNumTeeth();
    }
}
