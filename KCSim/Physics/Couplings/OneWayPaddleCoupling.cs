using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Mechanical;
using KCSim.Physics;

namespace KCSim.Physics.Couplings
{
    public class OneWayPaddleCoupling : Coupling
    {
        private Force inputToOutputForce;
        private Force outputToInputForce;

        private readonly double inputToOutputRatio;
        private readonly double outputToInputRatio;
        private readonly double sign;
        private readonly Paddle paddle;

        public OneWayPaddleCoupling(
            PaddleWheel paddleWheel,
            Paddle paddle,
            double inputToOutputRatio,
            Direction direction,
            string name = "") : base(paddleWheel, paddle, name)
        {
            this.inputToOutputRatio = inputToOutputRatio;
            this.outputToInputRatio = 1.0 / inputToOutputRatio;
            this.sign = direction.Sign();
            this.paddle = paddle;
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
                inputToOutputForce = Force.ZeroForce;
                return;
            }

            // If the paddle is in the engaged position, apply no more force.
            if (sign > 0 && paddle.GetPosition().Equals(Paddle.Position.Negative))
            {
                inputToOutputForce = Force.ZeroForce;
                return;
            }
            if (sign < 0 && paddle.GetPosition().Equals(Paddle.Position.Positive))
            {
                inputToOutputForce = Force.ZeroForce;
                return;
            }

            inputToOutputForce = new Force(force.Velocity * inputToOutputRatio * -1.0);
        }

        protected override void ReceiveForceOnOutput(Force force)
        {
            outputToInputForce = Force.ZeroForce;
        }
    }
}
