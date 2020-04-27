using System;
using KCSim;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.State
{
    public class DFlipFlopTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ForceEvaluator forceEvaluator;
        private readonly ICouplingService couplingService;

        private ExternalSwitch Power = new ExternalSwitch(new Force(1));
        private ExternalSwitch DataIn = new ExternalSwitch();
        private ExternalSwitch Clock = new ExternalSwitch();

        private readonly DFlipFlop flipFlop;

        public DFlipFlopTests()
        {
            couplingService = testUtil.GetSingletonCouplingService();
            forceEvaluator = testUtil.GetSingletonForceEvaluator();

            flipFlop = new DFlipFlop(
                couplingService: couplingService,
                gateFactory: testUtil.GetGateFactory(),
                stateFactory: testUtil.GetStateFactory());

            couplingService.CreateNewLockedCoupling(Power, flipFlop.Power);
            couplingService.CreateNewLockedCoupling(Clock, flipFlop.Clock);
            couplingService.CreateNewLockedCoupling(DataIn, flipFlop.Data);

            testUtil.InitializeState(flipFlop.Latches[0]);
            testUtil.InitializeState(flipFlop.Latches[1]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void TestThat_DataLatches_OnPositiveClockEdge(int dataToLatch)
        {
            DataIn.Force = new Force(dataToLatch);
            StateTestUtil.ToggleClockFrom_NegativeToPositive(Clock, forceEvaluator);
            StateTestUtil.ToggleClockFrom_NegativeToPositive(Clock, forceEvaluator);

            TestUtil.AssertDirectionsEqual(new Force(dataToLatch), flipFlop.Q.GetNetForce());
        }
    }
}
