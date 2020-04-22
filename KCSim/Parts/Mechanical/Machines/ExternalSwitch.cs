using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical.Machines
{
    public class ExternalSwitch : Torqueable
    {
        private Force force;

        public ExternalSwitch(string name = "") : base(name)
        {
        }

        public override KeyValuePair<Torqueable, Force> GetNetForceAndSource()
        {
            if (force == null)
            {
                throw new InvalidOperationException("attempt to get force from external switch before force has been set");
            }
            return new KeyValuePair<Torqueable, Force>(this, force);
        }

        public Force Force { get => force; set => force = value; }
    }
}
