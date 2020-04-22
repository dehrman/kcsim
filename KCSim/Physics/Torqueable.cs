using System;
using System.Collections.Generic;
using System.Linq;
using KCSim.Physics.Couplings;

namespace KCSim.Physics
{
    public class Torqueable
    {
        private readonly Dictionary<Torqueable, Force> forces;
        private readonly string name;

        public Torqueable(string name="")
        {
            this.forces = new Dictionary<Torqueable, Force>();
            this.name = name;
        }

        public Force GetNetForce()
        {
            return GetNetForceAndSource().Value;
        }
        
        public virtual KeyValuePair<Torqueable, Force> GetNetForceAndSource()
        {
            if (this.forces.Count == 0)
            {
                return new KeyValuePair<Torqueable, Force>(this, Force.ZeroForce);
            }
            return forces.OrderByDescending(kvp => kvp.Value).ElementAt(0);
        }

        public virtual bool UpdateForce(Torqueable source, Force force)
        {
            Force previousNetForce = GetNetForceAndSource().Value;
            forces[source] = force;
            Force newNetForce = GetNetForceAndSource().Value;
            return !newNetForce.Equals(previousNetForce);
        }

        public void RemoveAllForces()
        {
            forces.Clear();
        }

        public override string ToString()
        {
            return "Torqueable \"" + name + "\": {net force=" + GetNetForce() + "}";
        }
    }
}
