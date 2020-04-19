using System;
using System.Collections.Generic;
using System.Linq;
using KCSim.Physics.Couplings;

namespace KCSim.Physics
{
    public class Torqueable
    {
        private readonly Dictionary<Coupling, Force> couplingsToForces;
        private readonly string name;

        public Torqueable(string name="")
        {
            this.couplingsToForces = new Dictionary<Coupling, Force>();
            this.name = name;
        }

        public Force GetNetForce()
        {
            if (couplingsToForces.Count == 0)
            {
                return Force.ZeroForce;
            }
            var forces = couplingsToForces.Values.ToList();
            forces.Sort();
            return forces[forces.Count - 1];
        }

        /**
         * return true if the net force changed.
         */
        public virtual bool UpdateForce(Coupling coupling, Force force)
        {
            Force previousNetForce = GetNetForce();
            couplingsToForces[coupling] = force;
            return GetNetForce().Velocity != previousNetForce.Velocity;
        }

        public void RemoveAllForces()
        {
            Force previousNetForce = GetNetForce();
            couplingsToForces.Clear();
        }

        public override string ToString()
        {
            return "Torqueable \"" + name + "\": {net force=" + GetNetForce() + "}";
        }
    }
}
