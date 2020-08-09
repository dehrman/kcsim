using System;
using KCSim;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;
using KCSim.Physics;
using KCSimTests.Parts.Logical;
using Xunit;

namespace KCSimTests.Parts.State
{
    public class DFlipFlopTests : BaseGateTest
    {
        private ExternalSwitch Power = new ExternalSwitch(new Force(1));
        private ExternalSwitch DataIn = new ExternalSwitch();
        private ExternalSwitch Clock = new ExternalSwitch();

        private readonly DFlipFlop flipFlop;

        public DFlipFlopTests()
        {
            flipFlop = new DFlipFlop(
                couplingService: couplingService,
                gateFactory: gateFactory,
                stateFactory: testUtil.GetStateFactory());

            couplingService.CreateNewLockedCoupling(Power, flipFlop.Power);
            couplingService.CreateNewLockedCoupling(Clock, flipFlop.Clock);
            couplingService.CreateNewLockedCoupling(DataIn, flipFlop.Data);

            // testUtil.InitializeState(flipFlop.Latches[0]);
            // testUtil.InitializeState(flipFlop.Latches[1]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void TestThat_DataLatches_OnPositiveClockEdge(int dataToLatch)
        {
            DataIn.Force = new Force(dataToLatch);
            ToggleClockFrom_NegativeToPositive(Clock);
            ToggleClockFrom_NegativeToPositive(Clock);

            TestUtil.AssertDirectionsEqual(new Force(dataToLatch), flipFlop.Q.GetNetForce());
        }

        private void ToggleClockFrom_NegativeToPositive(ExternalSwitch clock)
        {
            clock.Force = new Force(-1);
            EvaluateForcesWithDelay();
            clock.Force = new Force(1);
            EvaluateForcesWithDelay();
        }
    }
}
