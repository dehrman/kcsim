using System;
namespace KCSim.Physics
{
    public enum CouplingType
    {
        BidirectionalOpposing = 0,
        BidirectionalSymmetrical = 1,
        OneWayPositive = 2, // only coupled when input is in positive direction
        OneWayNegative = 3, // only coupled when input is in negative direction
        FreeFlowing = 4, // coupled by a common axis but free-flowing in potentially opposing directions
    }

    public static class CouplingTypeExtensions
    {
        public static bool IsOpposing(this CouplingType couplingType)
        {
            return couplingType != CouplingType.BidirectionalSymmetrical;
        }

        public static bool IsBidirectional(this CouplingType couplingType)
        {
            return couplingType == CouplingType.BidirectionalOpposing || couplingType == CouplingType.BidirectionalSymmetrical;
        }

        public static bool IsOneWay(this CouplingType couplingType)
        {
            return couplingType == CouplingType.OneWayPositive || couplingType == CouplingType.OneWayNegative;
        }

        public static bool IsInputDrivingOutput(this CouplingType couplingType, double angularVelocity)
        {
            if (couplingType == CouplingType.FreeFlowing)
            {
                return false;
            }
            if (couplingType == CouplingType.OneWayPositive)
            {
                return angularVelocity > 0;
            }
            if (couplingType == CouplingType.OneWayNegative)
            {
                return angularVelocity < 0;
            }
            else
            {
                return true;
            }
        }
    }
}
