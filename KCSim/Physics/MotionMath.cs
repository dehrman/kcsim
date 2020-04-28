using System;
using System.Diagnostics.Contracts;

namespace KCSim.Physics
{
    public class MotionMath
    {
        public static bool IsSameDirection(Force force1, Force force2)
        {
            Contract.Requires(force1 != null);
            Contract.Requires(force2 != null);
            return IsSameDirection(force1.Velocity, force2.Velocity);
        }

        public static bool IsSameDirection(double v1, double v2)
        {
            if (v1 == v2)
            {
                return v1 != 0;
            }
            if (v1 == 0 || v2 == 0)
            {
                return false;
            }

            // If one is negative and the other is positive, then the direction is different.
            return (v1 / v2 > 0);
        }
    }
}
