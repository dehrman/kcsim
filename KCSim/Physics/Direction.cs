using System;
using System.Collections.Generic;
using System.Text;

namespace KCSim.Physics
{
    public enum Direction
    {
        Positive,
        Negative
    }

    public static class DirectionExtensions
    {
        public static double Sign(this Direction direction)
        {
            return direction == Direction.Positive ? 1.0 : -1.0;
        }

        public static Direction Opposite(this Direction direction)
        {
            return direction == Direction.Positive ? Direction.Negative : Direction.Positive;
        }
    }
}
