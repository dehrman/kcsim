using KCSim;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.State
{
    public class SRLatchTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ForceEvaluator forceEvaluator;
        private readonly ICouplingService couplingService;

        private ExternalSwitch Power = new ExternalSwitch(new Force(1));
        private ExternalSwitch SetInverse = new ExternalSwitch();
        private ExternalSwitch ResetInverse = new ExternalSwitch();

        private readonly SRLatch latch;

        public SRLatchTests()
        {
            couplingService = testUtil.GetSingletonCouplingService();
            forceEvaluator = testUtil.GetSingletonForceEvaluator();

            latch = testUtil.GetStateFactory().CreateNewSRLatch();

            couplingService.CreateNewLockedCoupling(Power, latch.Power);
            couplingService.CreateNewLockedCoupling(SetInverse, latch.SetInverse);
            couplingService.CreateNewLockedCoupling(ResetInverse, latch.ResetInverse);

            testUtil.InitializeState(latch);
        }

        [Theory]
        [InlineData(-1, 1, 1)]
        [InlineData(1, -1, -1)]
        public void TestThat_TruthTableHolds(int setInverse, int resetInverse, int expectedOutput)
        {
            SetInverse.Force = new Force(setInverse);
            ResetInverse.Force = new Force(resetInverse);
            forceEvaluator.EvaluateForces();

            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), latch.Q.GetNetForce());
            TestUtil.AssertDirectionsEqual(new Force(expectedOutput * -1), latch.QInverse.GetNetForce());
        }

        [Fact]
        public void TestThat_LatchHoldsValue_IfSetAndResetAreBothHigh()
        {
            SetInverse.Force = new Force(-1);
            ResetInverse.Force = new Force(1);
            forceEvaluator.EvaluateForces();
            SetInverse.Force = new Force(1);
            forceEvaluator.EvaluateForces();

            TestUtil.AssertDirectionsEqual(new Force(1), latch.Q.GetNetForce());
            TestUtil.AssertDirectionsEqual(new Force(-1), latch.QInverse.GetNetForce());
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public void TestThat_LatchSeesNewValue(int oldValue, int newValue)
        {
            // Get the system into a good initial state.
            forceEvaluator.EvaluateForces();

            // Latch in the initial value.
            SetInverse.Force = new Force(oldValue * -1);
            ResetInverse.Force = new Force(oldValue);
            forceEvaluator.EvaluateForces();

            // Hold the value.
            SetInverse.Force = new Force(1);
            ResetInverse.Force = new Force(1);
            forceEvaluator.EvaluateForces(); // There's a bug on this line; after the completion of this function, the second (lower) NAND gate reports an output of -1 despite its inputs being [-1, 1]

            // Latch in the new value.
            SetInverse.Force = new Force(newValue * -1);
            ResetInverse.Force = new Force(newValue);
            forceEvaluator.EvaluateForces();

            // Hold the value.
            SetInverse.Force = new Force(1);
            ResetInverse.Force = new Force(1);
            forceEvaluator.EvaluateForces();

            TestUtil.AssertDirectionsEqual(new Force(newValue), latch.Q.GetNetForce());
            TestUtil.AssertDirectionsEqual(new Force(newValue * -1), latch.QInverse.GetNetForce());
        }
    }
}
