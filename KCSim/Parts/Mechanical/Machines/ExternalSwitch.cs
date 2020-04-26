using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical.Machines
{
    public class ExternalSwitch : Torqueable
    {
        private readonly string name;
        private Force force = null;

        public ExternalSwitch(string name = "") : base(name)
        {
        }

        public ExternalSwitch(
            Force initialForce,
            string name = "") : base(name)
        {
            this.name = name;
            this.force = initialForce;
        }

        public override KeyValuePair<Torqueable, Force> GetNetForceAndSource()
        {
            if (force == null)
            {
                throw new InvalidOperationException("attempt to get force from external switch "
                    + ToString()
                    + " before force has been set");
            }
            return new KeyValuePair<Torqueable, Force>(this, force);
        }

        public Force Force { get => force; set => force = value; }

        public override string ToString()
        {
            return "External switch \"" + name + "\": {net force="
                   + (force == null ? "(null)" : force.ToString())
                   + "}";
        }
    }
}
