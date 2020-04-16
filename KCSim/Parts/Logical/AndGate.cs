using KCSim.Parts.Mechanical;

namespace KCSim.Parts.Logical
{
    class AndGate : BinaryGate
    {
        public readonly Axle InputPower;
        public readonly Axle InputA;
        public readonly Axle InputB;
        public readonly Axle Output;

        AndGate(IBidirectionalLatchFactory bidirectionalLatchFactory)
        {
            // Create a bidirectional latch for each input.
            BidirectionalLatch inputLatchA = bidirectionalLatchFactory.CreateNew();
            BidirectionalLatch inputLatchB = bidirectionalLatchFactory.CreateNew();

            // If inputA is false, inputLatchB is not powered....
            Coupling<Axle, Axle>.NewLockedAxleCoupling(inputLatchA.OutputAxlePositive, inputLatchB.InputPower);
            
            // But that's OK, because if inputA is false, the AND gate's output will still be false
            // because it's coupled to inputLatchA's negative output axle.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(inputLatchA.OutputAxleNegative, Output);
            
            // Now, assuming inputA is true, and inputLatchB is powered, we'll want the output to be powered
            // by either the positive or the negative output from inputLatchB. While this may seem like a
            // destructive coupling at first, it works because it's guaranteed by the physics of the latch
            // that its two outputs are never engaged at the same time. Therefore, any backpressure from one
            // output axle to another terminates roughly at the point at which the disengaged axle leaves the
            // relay.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(inputLatchB.OutputAxleNegative, Output);
            Coupling<Axle, Axle>.NewLockedAxleCoupling(inputLatchB.OutputAxlePositive, Output);
        }
    }
}
