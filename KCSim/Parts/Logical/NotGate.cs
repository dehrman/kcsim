using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class NotGate : Gate
    {
        public NotGate(
            ICouplingService couplingService) : base("NOT gate")
        {
            Gear inputGear = new MediumGear();
            couplingService.CreateNewLockedCoupling(Input, inputGear);
            Gear outputGear = new MediumGear();
            couplingService.CreateNewGearCoupling(inputGear, outputGear);
            couplingService.CreateNewLockedCoupling(outputGear, Output);
        }
    }
}
