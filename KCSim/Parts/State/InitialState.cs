using System;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class InitialState : Force
    {
        // Random initial velocity
        private const double defaultVelocity = 1d - 1d / Math.PI;

        public InitialState(double velocity = defaultVelocity) : base(velocity)
        {
            System.Diagnostics.Debug.Write(defaultVelocity);
        }

        public override string ToString()
        {
            return "initial state force";
        }
    }
}
