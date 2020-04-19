using System;
using KCSim.Parts.Mechanical.Atomic;
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

        public Diode(
            ICouplingService couplingService,
            Direction direction,
            string name = "default diode name")
        {
            this.isPositiveDirection = isPositiveDirection;
            this.name = name;

            InputAxle = new Axle(name + "; diode input axle");
            InputGear = new SmallGear(name + "; diode input gear");
            connector = new SmallGear(name + "; diode connector gear");
            OutputGear = new SmallGear(name + "; diode output gear");
            OutputAxle = new Axle(name + "; diode output axle");

            couplingService.CreateNewLockedCoupling(
                input: InputAxle,
                output: InputGear,
                name: "diode input axle to input gear");
            couplingService.CreateNewOneWayCoupling(
                input: InputGear,
                output: connector,
                direction: direction,
                name: "diode input gear to connector gear");
            couplingService.CreateNewOneWayCoupling(
                input: connector,
                output: OutputGear,
                direction: direction.Opposite(),
                name: "diode connector gear to output gear");
            couplingService.CreateNewLockedCoupling(
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
