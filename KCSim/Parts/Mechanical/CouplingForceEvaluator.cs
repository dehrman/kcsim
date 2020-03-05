using System;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical
{
    public class CouplingForceEvaluator<Input, Output>
        where Input : Torqueable
        where Output : Torqueable
    {
        private readonly CouplingType couplingType;
        private readonly double inputToOutputRatio;

        public CouplingForceEvaluator(CouplingType couplingType, double inputToOutputRatio)
        {
            this.couplingType = couplingType;
            this.inputToOutputRatio = inputToOutputRatio;
        }

        /**
         * <summary>
         * Evaluate forces from the provided input and output
         * </summary>
         * <param name="input">the input Torqueable</param>
         * <param name="output">the output Torqueable</param>
         * <returns>a Tuple of forces to be applied (a) from the input to the output and (b) from the output to the
         * input, respectively</returns>
         */
        public Tuple<Force, Force> EvaluateForces(Input input, Output output)
        {
            Force inputForce = input.GetNetForce();
            Force outputForce = output.GetNetForce();

            // Scale the velocities by the input-to-output ratio. For instance, if the input-to-output ratio is 1:2
            // (0.5) and the input velocity is 10, the scaledInputVelocity will be 5 (10 * 0.5).
            double scaledInputVelocity = inputForce.Velocity * inputToOutputRatio;
            double scaledOutputVelocity = outputForce.Velocity / inputToOutputRatio;

            ValidateNoDestructiveForcesOrThrow(scaledInputVelocity, scaledOutputVelocity);

            if (!IsCouplingEngaged(scaledInputVelocity))
            {
                return ZeroForcePair();
            }

            int directionalScaling = couplingType.IsOpposing() ? -1 : 1;

            if (couplingType.IsBidirectional())
            {
                // Let the strongest force win!
                if (Math.Abs(scaledInputVelocity) > Math.Abs(scaledOutputVelocity))
                {
                    return new Tuple<Force, Force>(new Force(scaledInputVelocity * directionalScaling), outputForce);
                }
                else if (Math.Abs(scaledInputVelocity) < Math.Abs(scaledOutputVelocity))
                {
                    return new Tuple<Force, Force>(inputForce, new Force(scaledOutputVelocity * directionalScaling));
                }
                else
                {
                    // The scaled, directionally adjusted forces are equal, so just return the original two forces.
                    return new Tuple<Force, Force>(inputForce, outputForce);
                }
            }

            if (couplingType.IsOneWay())
            {
                // In any case, the output force cannot apply to the input force (since it's one way).
                return new Tuple<Force, Force>(new Force(scaledInputVelocity * directionalScaling), Force.ZeroForce);
            }

            throw new ArgumentException("Unhandled force configuration—error in the force evaluation framework.");
        }

        private bool IsCouplingEngaged(double scaledInputVelocity)
        {
            if (couplingType == CouplingType.FreeFlowing)
            {
                return false;
            }

            if (couplingType == CouplingType.OneWayPositive && scaledInputVelocity < 0)
            {
                return false;
            }

            if (couplingType == CouplingType.OneWayNegative && scaledInputVelocity > 0)
            {
                return false;
            }

            return true;
        }

        private Tuple<Force, Force> ZeroForcePair()
        {
            return new Tuple<Force, Force>(Force.ZeroForce, Force.ZeroForce);
        }

        private void ValidateNoDestructiveForcesOrThrow(double inputVelocity, double outputVelocity)
        {
            if (couplingType == CouplingType.FreeFlowing)
            {
                return;
            }

            if (couplingType == CouplingType.BidirectionalOpposing
                && MotionMath.IsSameDirection(inputVelocity, outputVelocity)) {
                throw new DestructiveForceException(
                    "Symmetrical forces not allowed in bidirectional opposing coupling");
            }

            if (couplingType == CouplingType.BidirectionalSymmetrical
                && MotionMath.IsDifferentDirection(inputVelocity, outputVelocity))
            {
                throw new DestructiveForceException(
                    "Opposing forces not allowed in bidirectional symmetrical coupling");
            }

            if (couplingType == CouplingType.OneWayPositive && outputVelocity > 0)
            {
                throw new DestructiveForceException(
                    "Positive output force not allowed in one-way-positive coupling");
            }

            if (couplingType == CouplingType.OneWayNegative && outputVelocity < 0)
            {
                throw new DestructiveForceException(
                    "Negative output force not allowed in one-way-negative coupling");
            }
        }
    }
}
