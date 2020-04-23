using System;

namespace KCSim
{
    public class Level
    {
        private readonly double numericValue;

        public Level(double numericValue)
        {
            this.numericValue = numericValue;
        }

        public bool AsBool()
        {
            if (numericValue == 0)
            {
                throw new InvalidOperationException("level is in a nondeterministic state");
            }
            return numericValue > 0;
        }

        public bool IsPositive()
        {
            return numericValue > 0;
        }

        public bool IsNegative()
        {
            return numericValue < 0;
        }

        public bool IsNondeterministic()
        {
            return numericValue == 0;
        }
    }
}
