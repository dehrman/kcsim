using KCSim.Parts.Mechanical;

namespace KCSim.Parts.Logical
{
    public class AndGate : BinaryGate
    {
        public readonly Axle Power = new Axle("AND gate power");
        public readonly Axle InputA = new Axle("AND gate inputA");
        public readonly Axle InputB = new Axle("AND gate inputB");
        public readonly Axle Output = new Axle("AND gate output");

        public AndGate(IBidirectionalLatchFactory bidirectionalLatchFactory) : base("AND gate")
        {
            // Create a bidirectional latch for each input.
            BidirectionalLatch inputLatchA = bidirectionalLatchFactory.CreateNew("AND gate inputLatchA");
            BidirectionalLatch inputLatchB = bidirectionalLatchFactory.CreateNew("AND gate inputLatchB");

            // Connect the inputs to the latches.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(InputA, inputLatchA.Input);
            Coupling<Axle, Axle>.NewLockedAxleCoupling(InputB, inputLatchB.Input);

            // Connect the power to inputLatchA.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(Power, inputLatchA.Power);

            // If inputA is false, inputLatchB is not powered....
            Coupling<Axle, Axle>.NewLockedAxleCoupling(inputLatchA.OutputAxlePositive, inputLatchB.Power);
            
            // But that's OK, because if inputA is false, the AND gate's output will still be false
            // because it's coupled to inputLatchA's negative output axle.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(
                inputLatchA.OutputAxleNegative,
                Output,
                "coupling from inputLatchA's negative output to the AND gate's output");

            // Now, assuming inputA is true, and inputLatchB is powered, we'll want the output to be powered
            // by either the positive or the negative output from inputLatchB. While this may seem like a
            // destructive coupling at first, it works because it's guaranteed by the physics of the latch
            // that its two outputs are never engaged at the same time. Therefore, any backpressure from one
            // output axle to another terminates roughly at the point at which the disengaged axle leaves the
            // relay.
            Coupling<Axle, Axle>.NewLockedAxleCoupling(
                inputLatchB.OutputAxleNegative,
                Output,
                "coupling from inputLatchB's negative output to the And gate's output");
            Coupling<Axle, Axle>.NewLockedAxleCoupling(
                inputLatchB.OutputAxlePositive,
                Output,
                "coupling from inputLatchB's positive output the the AND gate's output");
        }
    }
}
