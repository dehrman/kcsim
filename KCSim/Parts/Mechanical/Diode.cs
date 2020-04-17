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

            InputAxle = new Axle(name + "; diode input axle");
            InputGear = new SmallGear(name + "; diode input gear");
            connector = new SmallGear(name + "; diode connector gear");
            OutputGear = new SmallGear(name + "; diode output gear");
            OutputAxle = new Axle(name + "; diode output axle");

            Coupling<Axle, SmallGear>.NewLockedAxleToGearCoupling(
                input: InputAxle,
                output: InputGear,
                name: "diode input axle to input gear");
            Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: InputGear,
                output: connector,
                couplingType: (isPositiveDirection ? CouplingType.OneWayPositive : CouplingType.OneWayNegative),
                name: "diode input gear to connector gear");
            Coupling<SmallGear, SmallGear>.NewGearCoupling(
                input: connector,
                output: OutputGear,
                couplingType: (isPositiveDirection ? CouplingType.OneWayNegative : CouplingType.OneWayPositive),
                name: "diode connector gear to output gear");
            Coupling<SmallGear, Axle>.NewLockedGearToAxleCoupling(
                input: OutputGear,
                output: OutputAxle,
                name: "diode output gear to output axle");
        }

        public override string ToString()
        {
            return "Diode: \"" + name + "\" = {isPositiveDirection: " + isPositiveDirection + "}";
        }
    }
}
