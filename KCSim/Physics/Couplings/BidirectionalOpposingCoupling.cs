using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;

namespace KCSim.Physics.Couplings
{
    class BidirectionalOpposingCoupling : Coupling
    {
        private Force inputToOutputForce;
        private Force outputToInputForce;

        private readonly double inputToOutputRatio;
        private readonly double outputToInputRatio;

        public BidirectionalOpposingCoupling(
            Torqueable input,
            Torqueable output,
            double inputToOutputRatio,
            string name = "") : base(input, output, name)
        {
            this.inputToOutputRatio = inputToOutputRatio;
            this.outputToInputRatio = 1.0 / inputToOutputRatio;
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
            inputToOutputForce = new Force(force.Velocity * inputToOutputRatio * -1.0);
        }

        protected override void ReceiveForceOnOutput(Force force)
        {
            outputToInputForce = new Force(force.Velocity * outputToInputRatio * -1.0);
        }
    }
}
