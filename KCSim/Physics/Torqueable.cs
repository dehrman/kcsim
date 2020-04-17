using System;
using System.Collections.Generic;

namespace KCSim.Physics
{
    public class Torqueable
    {
        private readonly List<Force> forces;
        private readonly string name;

        public delegate void TorqueChangedDelegate(Torqueable torqueable);

        public readonly ISet<TorqueChangedDelegate> OnNetForceChangedDelegateSet
            = new HashSet<TorqueChangedDelegate>();
        public readonly ISet<TorqueChangedDelegate> OnDirectionChangedDelegateSet
            = new HashSet<TorqueChangedDelegate>();

        public Torqueable(string name="")
        {
            this.forces = new List<Force>();
            this.name = name;
        }

        public Force GetNetForce()
        {
            if (forces.Count == 0)
            {
                return Force.ZeroForce;
            }
            return forces[forces.Count - 1];
        }

        public void RemoveForce(Force force)
        {
            Force previousNetForce = GetNetForce();
            forces.Remove(force);
            CheckAndNotifyForNetForceChange(previousNetForce);
        }

        public void RemoveAllForces()
        {
            Force previousNetForce = GetNetForce();
            forces.Clear();
            CheckAndNotifyForNetForceChange(previousNetForce);
        }

        public virtual void AddForce(Force force)
        {
            if (forces.Contains(force))
            {
                return;
            }

            Force previousNetForce = GetNetForce();

            // Check that we're not applying a destructive force.
            if (MotionMath.IsDifferentDirection(previousNetForce.Velocity, force.Velocity))
            {
                throw new ArgumentException("Can't apply force " + force + " when counteracting force of "
                    + previousNetForce + " exists on " + ToString());
            }
            
            forces.Add(force);
            forces.Sort();
            CheckAndNotifyForNetForceChange(previousNetForce);
            CheckAndNotifyForDirectionChange(previousNetForce);
        }

        /**
         * <returns>true if the net force changed, false otherwise</returns>
         */
        protected virtual bool CheckAndNotifyForNetForceChange(Force previousNetForce)
        {
            Force netForce = GetNetForce();
            if (previousNetForce.Equals(netForce))
            {
                return false;
            }

            // We must create a copy of the delegates before iterating through them in the event that the set is
            // modified from outside this class during the iteration.
            ISet<TorqueChangedDelegate> copyOfDelegates =
                new HashSet<TorqueChangedDelegate>(OnNetForceChangedDelegateSet);
            foreach (TorqueChangedDelegate handler in copyOfDelegates)
            {
                handler(this);
            }

            return true;
        }

        /**
         * <returns>true if the direction changed, false otherwise</returns>
         */
        protected virtual bool CheckAndNotifyForDirectionChange(Force previousNetForce)
        {
            Force netForce = GetNetForce();
            if (MotionMath.IsSameDirection(previousNetForce.Velocity, netForce.Velocity))
            {
                return false;
            }

            // We must create a copy of the delegates before iterating through them in the event that the set is
            // modified from outside this class during the iteration.
            ISet<TorqueChangedDelegate> copyOfDelegates =
                new HashSet<TorqueChangedDelegate>(OnDirectionChangedDelegateSet);
            foreach (TorqueChangedDelegate handler in copyOfDelegates)
            {
                handler(this);
            }

            return true;
        }

        public override string ToString()
        {
            return "Torqueable \"" + name + "\": {net force=" + GetNetForce() + "}";
        }
    }
}
