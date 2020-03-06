using System;
namespace KCSim.Physics
{
    public class MotionMath
    {
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

        public static bool IsDifferentDirection(double v1, double v2)
        {
            if (v1 == 0 || v2 == 0)
            {
                return false;
            }

            // If one is negative and the other is positive, then the direction is different.
            return (v1 / v2 < 0);
        }
    }
}
