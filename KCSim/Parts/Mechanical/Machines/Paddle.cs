using System;
using System.Collections.Generic;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim.Parts.Mechanical
{
    public class Paddle : Gear
    {
        public enum Position
        {
            Negative = -1,
            Intermediate = 0,
            Positive = 1
        }

        public delegate void PaddleChangedDelegate(Paddle paddle, Position position);
        public ISet<PaddleChangedDelegate> OnPaddlePositionChangedDelegateSet =
            new HashSet<PaddleChangedDelegate>();

        private const double IntermediateRestingPlaceDegrees = 90;
        private const double NegativeRestingPlaceDegrees = IntermediateRestingPlaceDegrees - 45;
        private const double PositiveRestingPlaceDegrees = IntermediateRestingPlaceDegrees + 45;
        
        private double currentAngle;
        private readonly IMotionTimer motionTimer;

        public Paddle(
            IMotionTimer motionTimer,
            Position initialPosition = Position.Positive,
            string name = "")
            : base (name)
        {
            if (initialPosition == Position.Negative)
            {
                currentAngle = NegativeRestingPlaceDegrees;
            }
            else if (initialPosition == Position.Intermediate)
            {
                currentAngle = IntermediateRestingPlaceDegrees;
            }
            else
            {
                currentAngle = PositiveRestingPlaceDegrees;
            }

            this.motionTimer = motionTimer;
        }

        public override bool UpdateForce(Torqueable source, Force force)
        {
            Force oldForce = GetNetForce();
            bool isForceDifferent = base.UpdateForce(source, force);
            
            // If there's no change in force, break early.
            if (!isForceDifferent)
            {
                return false;
            }

            // If the direction changed, reset the motion timer.
            if (!MotionMath.IsSameDirection(oldForce.Velocity, GetNetForce().Velocity))
            {
                currentAngle = IntermediateRestingPlaceDegrees;
                ReinitializeTimer();
            }
            
            return true;
        }

        public override uint GetNumTeeth()
        {
            return 1;
        }

        private void ReinitializeTimer()
        {
            motionTimer.Stop();

            double velocity = GetNetForce().Velocity;
            if (velocity == 0)
            {
                return;
            }

            double degreesToStoppingPlace = GetDestination(velocity) - currentAngle;
            motionTimer.Start(degreesToStoppingPlace, velocity, OnMotionTimerCompletion);
        }

        // When the motion timer completes, set the current angle of the paddle to its destination.
        private void OnMotionTimerCompletion(double velocity)
        {
            currentAngle = GetDestination(velocity);

            // The paddle has been pushed to its destination, out of the range of its corresponding paddle wheel, so
            // all forces should be removed.
            RemoveAllForces();

            NotifyPaddlePositionChanged();
        }

        private void NotifyPaddlePositionChanged()
        {
            // We must create a copy of the delegates before iterating through them in the event that the set is
            // modified from outside this class during the iteration.
            ISet<PaddleChangedDelegate> copyOfDelegates =
                new HashSet<PaddleChangedDelegate>(OnPaddlePositionChangedDelegateSet);
            foreach (PaddleChangedDelegate handler in copyOfDelegates)
            {
                handler(this, GetPosition());
            }
        }

        // Here we're using the right hand rule to compute the destination angle given an angular velocity. If the
        // angular velocity is positive, pointed out of the plane (toward the viewer), it yields positive
        // degrees swept away from zero. If the angular velocity is negative, it yields negative degrees swept away
        // from zero (that is moving clockwise back toward zero degrees).
        private static double GetDestination(double velocity)
        {
            return velocity < 0 ? NegativeRestingPlaceDegrees : PositiveRestingPlaceDegrees;
        }

        private Position GetPosition()
        {
            switch (currentAngle)
            {
                case IntermediateRestingPlaceDegrees:
                    return Position.Intermediate;
                case NegativeRestingPlaceDegrees:
                    return Position.Negative;
                case PositiveRestingPlaceDegrees:
                    return Position.Positive;
                default:
                    throw new InvalidOperationException("GetPosition invoked during non-determinate paddle state");
            }
        }
    }
}
