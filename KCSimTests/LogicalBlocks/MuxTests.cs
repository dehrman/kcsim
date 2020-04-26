using System.Linq;
using KCSim;
using KCSim.LogicalBlocks;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.LogicalBlocks
{
    public class MuxTests
    {
        private readonly TestUtil testUtil;
        private readonly ICouplingService couplingService;
        private readonly IGateFactory gateFactory;
        private readonly LogicalBlockFactory logicalBlockFactory;
        private readonly ForceEvaluator forceEvaluator;

        private readonly ExternalSwitch motor = new ExternalSwitch(new Force(1));

        public MuxTests()
        {
            testUtil = new TestUtil();
            couplingService = testUtil.GetSingletonCouplingService();
            gateFactory = testUtil.GetGateFactory();
            logicalBlockFactory = new LogicalBlockFactory(couplingService, gateFactory);
            forceEvaluator = testUtil.GetSingletonForceEvaluator();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        public void TestThat_Something(int numInputs)
        {
            var mux = new Mux(couplingService, gateFactory, numInputs);
            couplingService.CreateNewLockedCoupling(motor, mux.Power);

            // Connect the select lines to the mux.
            int numSelectBits = BitMath.GetNumSelectBitsRequired(numInputs);
            ExternalSwitch[] selectLines = new ExternalSwitch[numSelectBits];
            for (int i = 0; i < numSelectBits; i++)
            {
                selectLines[i] = new ExternalSwitch();
                couplingService.CreateNewLockedCoupling(selectLines[i], mux.Select[i]);
            }

            // Populate an array of inputs.
            ExternalSwitch[] inputs = new ExternalSwitch[numInputs];
            for (int i = 0; i < numInputs; i++)
            {
                inputs[i] = new ExternalSwitch();
                couplingService.CreateNewLockedCoupling(inputs[i], mux.Inputs[i]);
            }

            // Test that when its select line matches 
            for (int inputToExpectOnOutput = 0; inputToExpectOnOutput < numInputs; inputToExpectOnOutput++)
            {
                Enumerable.Range(0, numInputs).Select(i => inputs[i].Force = Force.ZeroForce);
                inputs[inputToExpectOnOutput].Force = new Force(1);
                bool[] shouldSetSelectLine = BitMath.GetBitVector(numSelectBits, inputToExpectOnOutput);
                Enumerable.Range(0, numSelectBits)
                    .Select(i => selectLines[i].Force = new Force(shouldSetSelectLine[i] ? 1 : -1));

                forceEvaluator.EvaluateForces();

                TestUtil.AssertDirectionsEqual(inputs[inputToExpectOnOutput].GetNetForce(), mux.Output.GetNetForce());
            }
        }
    }
}
