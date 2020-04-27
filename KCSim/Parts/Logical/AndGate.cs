using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class AndGate : BinaryInputGate
    {
        public AndGate(
            ICouplingService couplingService,
            IBidirectionalLatchFactory bidirectionalLatchFactory,
            string name = "AND gate")
            : base(name)
        {
            // Create a bidirectional latch for each input.
            BidirectionalLatch inputLatchA = bidirectionalLatchFactory.CreateNew(name + "; inputLatchA");
            BidirectionalLatch inputLatchB = bidirectionalLatchFactory.CreateNew(name + "; inputLatchB");

            // Connect the inputs to the latches.
            couplingService.CreateNewLockedCoupling(InputA, inputLatchA.Input);
            couplingService.CreateNewLockedCoupling(InputB, inputLatchB.Input);

            // Connect the power to inputLatchA.
            couplingService.CreateNewLockedCoupling(Power, inputLatchA.Power);

            // If inputA is false, inputLatchB is not powered....
            couplingService.CreateNewLockedCoupling(inputLatchA.OutputAxlePositive, inputLatchB.Power);
            
            // But that's OK, because if inputA is false, the AND gate's output will still be false
            // because it's coupled to inputLatchA's negative output axle.
            couplingService.CreateNewLockedCoupling(
                inputLatchA.OutputAxleNegative,
                Output,
                "coupling from inputLatchA's negative output to " + name + "'s output");

            // Now, assuming inputA is true, and inputLatchB is powered, we'll want the output to be powered
            // by either the positive or the negative output from inputLatchB. While this may seem like a
            // destructive coupling at first, it works because it's guaranteed by the physics of the latch
            // that its two outputs are never engaged at the same time. Therefore, any backpressure from one
            // output axle to another terminates roughly at the point at which the disengaged axle leaves the
            // relay.
            couplingService.CreateNewLockedCoupling(
                inputLatchB.OutputAxleNegative,
                Output,
                "coupling from inputLatchB's negative output to " + name + "'s output");
            couplingService.CreateNewLockedCoupling(
                inputLatchB.OutputAxlePositive,
                Output,
                "coupling from inputLatchB's positive output the " + name + "'s output");
        }

        public override bool RequiresPower()
        {
            return true;
        }
    }
}
