using System;
using System.Collections.Generic;
using System.Text;

namespace KCSim.Physics.Couplings
{
    class FreeFlowingCoupling : Coupling
    {
        public FreeFlowingCoupling(Torqueable input, Torqueable output, string name = "") : base(input, output, name)
        {
        }

        protected override Force GetInputForceOut()
        {
            return Force.ZeroForce;
        }

        protected override Force GetOutputForceOut()
        {
            return Force.ZeroForce;
        }

        protected override void ReceiveForceOnInput(Force force)
        {
        }

        protected override void ReceiveForceOnOutput(Force force)
        {
        }
    }
}
