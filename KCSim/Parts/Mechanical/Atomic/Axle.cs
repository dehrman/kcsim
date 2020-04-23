using System;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical.Atomic
{
    public class Axle : Rod
    {
        public delegate void OnForceChangeDelegate(Force oldForce, Force newForce);
        public OnForceChangeDelegate OnForceChange = null;

        public Axle(string name = "", OnForceChangeDelegate onForceChange = null) : base(name)
        {
            this.OnForceChange = onForceChange;
        }

        public override bool UpdateForce(Torqueable source, Force force)
        {
            Force oldForce = GetNetForce();
            if (!base.UpdateForce(source, force))
            {
                return false;
            }
            if (OnForceChange != null)
            {
                Force newForce = GetNetForce();
                OnForceChange.Invoke(oldForce, newForce);
            }
            return true;
        }
    }
}
