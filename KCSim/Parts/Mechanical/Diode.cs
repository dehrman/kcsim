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

        private readonly SmallGear connector;
        private readonly bool isPositiveDirection;
        private readonly string name;

        public Diode(bool isPositiveDirection = true, string name = "default diode name")
        {
            this.isPositiveDirection = isPositiveDirection;
            this.name = name;

            InputAxle = new Axle("input axle");
            InputGear = new SmallGear("input gear");
            connector = new SmallGear("connector gear");
            OutputGear = new SmallGear("output gear");
            OutputAxle = new Axle("output axle");

            Coupling<Axle, SmallGear>.NewLockedAxleToGearCoupling(
                input: InputAxle,
                output: InputGear,
                name: "input axle to input gear");
            Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: InputGear,
                output: connector,
                couplingType: (isPositiveDirection ? CouplingType.OneWayPositive : CouplingType.OneWayNegative),
                name: "input gear to connector gear");
            Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: connector,
                output: OutputGear,
                couplingType: (isPositiveDirection ? CouplingType.OneWayNegative : CouplingType.OneWayPositive),
                name: "connector gear to output gear");
            Coupling<SmallGear, Axle>.NewLockedGearToAxleCoupling(
                input: OutputGear,
                output: OutputAxle,
                name: "output gear to output axle");
        }

        public override string ToString()
        {
            return "Diode: \"" + name + "\" = {isPositiveDirection: " + isPositiveDirection + "}";
        }
    }
}
