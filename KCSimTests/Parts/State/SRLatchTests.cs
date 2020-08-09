using KCSim;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;
using KCSim.Physics;
using KCSimTests.Parts.Logical;
using Xunit;

namespace KCSimTests.Parts.State
{
    public class SRLatchTests : BaseGateTest
    {
        private ExternalSwitch Power = new ExternalSwitch(new Force(1 * 1000), "power");
        private ExternalSwitch Set = new ExternalSwitch("set");
        private ExternalSwitch Reset = new ExternalSwitch("reset");

        private readonly SRLatch latch;

        public SRLatchTests()
        {
            latch = testUtil.GetStateFactory().CreateNewSRLatch();

            couplingService.CreateNewLockedCoupling(Power, latch.Power);
            couplingService.CreateNewLockedCoupling(Set, latch.Set);
            couplingService.CreateNewLockedCoupling(Reset, latch.Reset);

            // testUtil.InitializeState(latch);
        }

        [Theory]
        [InlineData(1, -1, 1)]
        [InlineData(-1, 1, -1)]
        public void TestThat_TruthTableHolds(int set, int reset, int expectedOutput)
        {
            Set.Force = new Force(set * 1000);
            Reset.Force = new Force(reset * 1000);
            EvaluateForcesWithDelay();

            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), latch.Q.GetNetForce());
        }

        [Fact]
        public void TestThat_LatchHoldsValue_IfSetAndResetAreBothLow()
        {
            Set.Force = new Force(1 * 1000);
            Reset.Force = new Force(-1 * 1000);
            EvaluateForcesWithDelay();
            Set.Force = new Force(-1 * 1000);
            EvaluateForcesWithDelay();

            TestUtil.AssertDirectionsEqual(new Force(1), latch.Q.GetNetForce());
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public void TestThat_LatchSeesNewValue(int oldValue, int newValue)
        {
            // Latch in the initial value.
            Set.Force = new Force(oldValue * 1000);
            Reset.Force = new Force(oldValue * -1 * 1000);
            EvaluateForcesWithDelay();

            // Hold the value.
            Set.Force = new Force(-1 * 1000);
            Reset.Force = new Force(-1 * 1000);
            EvaluateForcesWithDelay();

            // Latch in the new value.
            Set.Force = new Force(newValue * 1000);
            Reset.Force = new Force(newValue * -1 * 1000);
            EvaluateForcesWithDelay();

            // Hold the value.
            Set.Force = new Force(-1 * 1000);
            Reset.Force = new Force(-1 * 1000);
            EvaluateForcesWithDelay();

            TestUtil.AssertDirectionsEqual(new Force(newValue * 1000), latch.Q.GetNetForce());
        }
    }
}
