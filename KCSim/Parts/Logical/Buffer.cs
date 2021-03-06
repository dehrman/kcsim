﻿using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    /**
     * A buffer is used to amplify a weak (i.e. low-torque) signal to a level suitable for driving
     * the inputs of subsequent gates.
     * 
     * Here, a buffer is implemented via a latch.
     */
    public class Buffer : Gate
    {
        public readonly Axle Input;
        public readonly Axle Output;

        public Buffer(
            ICouplingService couplingService,
            IBidirectionalLatchFactory bidirectionalLatchFactory,
            string name = "buffer") : base(name)
        {
            // Create the I/O.
            Input = new Axle(name + " input");
            Output = new Axle(name + " output");

            var latch = bidirectionalLatchFactory.CreateNew();
            couplingService.CreateNewLockedCoupling(Input, latch.Input);
            couplingService.CreateNewLockedCoupling(Power, latch.Power);

            // We can wire together both the positive and negative outputs of the latch because
            // it is inherent in the physics of the latch that only one will be powered at any
            // given time, with the other in a "high-impedance" state.
            var combinedOutput = new Axle();
            couplingService.CreateNewLockedCoupling(latch.OutputAxlePositive, combinedOutput);
            couplingService.CreateNewLockedCoupling(latch.OutputAxleNegative, combinedOutput);
            
            couplingService.CreateNewLockedCoupling(combinedOutput, Output);
        }

        public override bool RequiresPower()
        {
            return true;
        }
    }
}
