using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Physics;

namespace KCSim.Physics.Couplings
{
    class OneWayPaddleCoupling : Coupling
    {
        private Force inputToOutputForce;
        private Force outputToInputForce;

        private readonly double inputToOutputRatio;
        private readonly double outputToInputRatio;
        private readonly double sign;

        public OneWayPaddleCoupling(
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
            // If the force on the input gear is the opposite of the direction of this coupling,
            // the force does NOT propagate to the output gear. 
            if (force.Velocity * sign <= 0)
            {
                // One-way coupling does not engage in this case.
                inputToOutputForce = Force.ZeroForce;
            }
            else
            {
                inputToOutputForce = new Force(force.Velocity * inputToOutputRatio * -1.0);
            }
        }

        protected override void ReceiveForceOnOutput(Force force)
        {
            outputToInputForce = Force.ZeroForce;
        }
    }
}
