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

        private ExternalSwitch Power = new ExternalSwitch(new Force(1), "power");
        private ExternalSwitch Set = new ExternalSwitch("set");
        private ExternalSwitch Reset = new ExternalSwitch("reset");

        private readonly SRLatch latch;

        public SRLatchTests()
        {
            couplingService = testUtil.GetSingletonCouplingService();
            forceEvaluator = testUtil.GetSingletonForceEvaluator();

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
            Set.Force = new Force(set);
            Reset.Force = new Force(reset);
            forceEvaluator.EvaluateForces();

            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), latch.Q.GetNetForce());
        }

        [Fact]
        public void TestThat_LatchHoldsValue_IfSetAndResetAreBothLow()
        {
            Set.Force = new Force(1);
            Reset.Force = new Force(-1);
            forceEvaluator.EvaluateForces();
            Set.Force = new Force(-1);
            forceEvaluator.EvaluateForces();

            TestUtil.AssertDirectionsEqual(new Force(1), latch.Q.GetNetForce());
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public void TestThat_LatchSeesNewValue(int oldValue, int newValue)
        {
            // Latch in the initial value.
            Set.Force = new Force(oldValue);
            Reset.Force = new Force(oldValue * -1);
            forceEvaluator.EvaluateForces();

            // Hold the value.
            Set.Force = new Force(-1);
            Reset.Force = new Force(-1);
            forceEvaluator.EvaluateForces();

            // Latch in the new value.
            Set.Force = new Force(newValue);
            Reset.Force = new Force(newValue * -1);
            forceEvaluator.EvaluateForces();

            // Hold the value.
            Set.Force = new Force(-1);
            Reset.Force = new Force(-1);
            forceEvaluator.EvaluateForces();

            TestUtil.AssertDirectionsEqual(new Force(newValue), latch.Q.GetNetForce());
        }
    }
}
