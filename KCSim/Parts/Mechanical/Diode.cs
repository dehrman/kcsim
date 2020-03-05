using System;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical
{
    public class Diode
    {
        public readonly Axle InputAxle;
        public readonly SmallGear InputGear;
        public readonly SmallGear OutputGear;
        public readonly Axle OutputAxle;

        private SmallGear connector;
        private Coupling<Axle, SmallGear> inputAxleToInputGearCoupling;
        private Coupling<SmallGear, SmallGear> inputToConnectorCoupling;
        private Coupling<SmallGear, SmallGear> connectorToOutputCoupling;
        private Coupling<SmallGear, Axle> outputGearToOutputAxleCoupling;
        private readonly bool isPositiveDirection;

        public Diode(bool isPositiveDirection)
        {
            this.isPositiveDirection = isPositiveDirection;

            InputAxle = new Axle("input axle");
            InputGear = new SmallGear("input gear");
            connector = new SmallGear("connector gear");
            OutputGear = new SmallGear("output gear");
            OutputAxle = new Axle("output axle");

            inputAxleToInputGearCoupling = Coupling<Axle, SmallGear>.NewLockedAxleToGearCoupling(
                input: InputAxle,
                output: InputGear,
                name: "input axle to input gear");
            inputToConnectorCoupling = Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: InputGear,
                output: connector,
                couplingType: (isPositiveDirection ? CouplingType.OneWayPositive : CouplingType.OneWayNegative),
                name: "input gear to connector gear");
            connectorToOutputCoupling = Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: connector,
                output: OutputGear,
                couplingType: (isPositiveDirection ? CouplingType.OneWayNegative : CouplingType.OneWayPositive),
                name: "connector gear to output gear");
            outputGearToOutputAxleCoupling = Coupling<SmallGear, Axle>.NewLockedGearToAxleCoupling(
                input: OutputGear,
                output: OutputAxle,
                name: "output gear to output axle");
        }
    }
}
