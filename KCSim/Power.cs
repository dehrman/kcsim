using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;

namespace KCSim
{
    /// <summary>
    /// Power intended to be supplied to any parts in the computer requiring power.
    /// </summary>
    public class Power : ExternalSwitch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Power"/> class.
        /// </summary>
        /// <param name="initialForce">The power force.</param>
        public Power(Force initialForce) : base(initialForce: initialForce, name: "Power")
        {

        }
    }
}
