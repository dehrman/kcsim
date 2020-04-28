using System;
using System.Collections.Generic;
using System.Linq;
using KCSim.Parts.State;

namespace KCSim.Physics
{
    public class Torqueable
    {
        public delegate void OnForceChangeDelegate(Force oldForce, Force newForce);
        private readonly ISet<OnForceChangeDelegate> onForceChangeDelegates = new HashSet<OnForceChangeDelegate>();
        public event OnForceChangeDelegate OnForceChange
        {
            add { onForceChangeDelegates.Add(value); }
            remove { onForceChangeDelegates.Remove(value); }
        }

        private readonly Dictionary<Torqueable, Force> forces;
        private readonly string name;

        public Torqueable(string name = "")
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
            return forces
                .OrderByDescending(kvp => kvp.Value)
                .ElementAt(0);
        }

        public virtual bool UpdateForce(Torqueable source, Force force)
        {
            ValidateNoDestructiveForces(source, force);
            Force previousNetForce = GetNetForceAndSource().Value;
            forces[source] = force;
            Force newNetForce = GetNetForceAndSource().Value;

            if (newNetForce.Equals(previousNetForce))
            {
                return false;
            }

            // The net force has changed, so invoke the callback if one is available.
            // We must create a copy of the delegates before iterating through them in the event that the set is
            // modified from outside this class during the iteration.
            ISet<OnForceChangeDelegate> copyOfDelegates =
                new HashSet<OnForceChangeDelegate>(onForceChangeDelegates);
            foreach (OnForceChangeDelegate handler in copyOfDelegates)
            {
                handler(previousNetForce, newNetForce);
            }
            return true;
        }

        public void RemoveAllForces()
        {
            forces.Clear();
        }

        private void ValidateNoDestructiveForces(Torqueable source, Force force)
        {
            if (force.Equals(Force.ZeroForce))
            {
                return;
            }

            if (forces.Count == 0)
            {
                return;
            }

            IList<Force> preExistingForcesNotFromThisSource =
                forces.Where(kvp => !kvp.Key.Equals(source))
                    .Where(kvp => !kvp.Value.Equals(Force.ZeroForce))
                    .Select(kvp => kvp.Value)
                    .ToList();

            if (preExistingForcesNotFromThisSource.Count == 0)
            {
                return;
            }

            // We can pick any item from the list because by virtue of this function getting called, we're ensuring
            // that no forces going in different directions can end up in the map of forces.
            if (MotionMath.IsSameDirection(force, preExistingForcesNotFromThisSource.First()))
            {
                return;
            }

            throw new DestructiveForceException("Attempted to apply destructive force of "
                                                + force
                                                + " from "
                                                + source
                                                + " to "
                                                + this);
        }

        public override string ToString()
        {
            return "Torqueable \"" + name + "\": {net force=" + GetNetForce() + "}";
        }
    }
}
