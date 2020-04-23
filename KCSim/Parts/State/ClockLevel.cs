using System.Collections.Generic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class ClockLevel : Torqueable
    {
        private Force level;

        public override KeyValuePair<Torqueable, Force> GetNetForceAndSource()
        {
            return new KeyValuePair<Torqueable, Force>(this, level);
        }

        public override bool UpdateForce(Torqueable source, Force force)
        {
            return false;
        }

        public void Update(Force level)
        {
            this.level = level;
        }
    }
}
