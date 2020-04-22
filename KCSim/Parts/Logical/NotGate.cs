using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class NotGate
    {
        public readonly Axle Input = new Axle("NOT gate input");
        public readonly Axle Output = new Axle("NOT gate output");

        public NotGate(
            ICouplingService couplingService)
        {
            Gear inputGear = new MediumGear();
            couplingService.CreateNewLockedCoupling(Input, inputGear);
            Gear outputGear = new MediumGear();
            couplingService.CreateNewGearCoupling(inputGear, outputGear);
            couplingService.CreateNewLockedCoupling(outputGear, Output);
        }
    }
}
