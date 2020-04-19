using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;

namespace KCSim.Physics.Couplings
{
    class OneWayCoupling : Coupling
    {
        private Force inputToOutputForce;
        private Force outputToInputForce;

        private readonly double inputToOutputRatio;
        private readonly double outputToInputRatio;
        private readonly double sign;

        public OneWayCoupling(
            Torqueable input,
            Torqueable output,
            double inputToOutputRatio,
            Direction direction,
            string name = "") : base(input, output, name)
        {
            this.inputToOutputRatio = inputToOutputRatio;
            this.outputToInputRatio = 1.0 / inputToOutputRatio;
            this.sign = direction.Sign();
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
            if (outputToInputForce.Velocity * sign < 0)
            {
                outputToInputForce = new Force(force.Velocity * outputToInputRatio * -1.0);
            }
            else
            {
                outputToInputForce = Force.ZeroForce;
            }
        }
    }
}
