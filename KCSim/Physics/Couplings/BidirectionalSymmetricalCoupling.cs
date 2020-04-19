using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;

namespace KCSim.Physics.Couplings
{
    class BidirectionalSymmetricalCoupling : Coupling
    {
        private Force inputToOutputForce;
        private Force outputToInputForce;

        public BidirectionalSymmetricalCoupling(
            Torqueable input,
            Torqueable output,
            string name = "") : base(input, output, name)
        {
        }

        protected override Force GetInputForceOut()
        {
            return outputToInputForce;
        }

        protected override Force GetOutputForceOut()
        {
            return inputToOutputForce;
        }

        protected override void ReceiveForceOnInput(Force force)
        {
            inputToOutputForce = new Force(force.Velocity);
        }

        protected override void ReceiveForceOnOutput(Force force)
        {
            outputToInputForce = new Force(force.Velocity);
        }
    }
}
