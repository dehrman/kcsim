using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class NotGate : Gate
    {
        public readonly Axle Input;
        public readonly Axle Output;

        public NotGate(
            ICouplingService couplingService,
            string name = "NOT gate") : base(name)
        {
            // Create the I/O.
            Input = new Axle(name + " input");
            Output = new Axle(name + " output");

            Gear inputGear = new MediumGear();
            couplingService.CreateNewLockedCoupling(Input, inputGear);
            Gear outputGear = new MediumGear();
            couplingService.CreateNewGearCoupling(inputGear, outputGear);
            couplingService.CreateNewLockedCoupling(outputGear, Output);
        }

        public override bool RequiresPower()
        {
            return false;
        }
    }
}
