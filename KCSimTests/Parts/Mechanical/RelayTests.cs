using System.Threading;
using KCSim;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Mechanical
{
    public class RelayTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ForceEvaluator forceEvaluator;
        private readonly ICouplingMonitor couplingMonitor;

        private readonly ExternalSwitch enable = new ExternalSwitch(name: "enable switch");
        private readonly ExternalSwitch input = new ExternalSwitch(name: "input switch");

        public RelayTests()
        {
            forceEvaluator = testUtil.GetSingletonForceEvaluator();
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        public void TestThat_WhenNoForceIsOnControl_OutputStaysZero(int enableForce, int latchedInputForce)
        {
            enable.Force = Force.ZeroForce;
            input.Force = GetLatchedInputForce(latchedInputForce);
            var relay = GetRelay(enableForce, latchedInputForce);
            EvalutateForcesWithDelay();
            Assert.Equal(Force.ZeroForce, relay.OutputAxle.GetNetForce());
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        public void TestThat_WhenEnabledControl_OutputIsPowered(int enableForce, int latchedInputForce)
        {
            enable.Force = GetEnableForce(enableForce);
            input.Force = GetLatchedInputForce(latchedInputForce);
            var relay = GetRelay(enableForce, latchedInputForce);
            EvalutateForcesWithDelay();
            Assert.Equal(GetLatchedInputForce(latchedInputForce), relay.OutputAxle.GetNetForce());
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        public void TestThat_WhenDisabledControl_OutputIsNotPowered(int enableForce, int latchedInputForce)
        {
            enable.Force = GetDisableForce(enableForce);
            input.Force = GetLatchedInputForce(latchedInputForce);
            var relay = GetRelay(enableForce, latchedInputForce);
            EvalutateForcesWithDelay();
            Assert.Equal(Force.ZeroForce, relay.OutputAxle.GetNetForce());
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        public void TestThat_WhenRelayIsEngaged_ButInputIsNotLatched_OutputIsNotPowered(int enableForce, int latchedInputForce)
        {
            enable.Force = GetEnableForce(enableForce);
            input.Force = GetNonLatchedInputForce(latchedInputForce);
            var relay = GetRelay(enableForce, latchedInputForce);
            EvalutateForcesWithDelay();
            Assert.Equal(Force.ZeroForce, relay.OutputAxle.GetNetForce());
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        public void TestThat_RelayStaysEngagedWhenControlForceIsRemoved(int enableForce, int latchedInputForce)
        {
            enable.Force = GetEnableForce(enableForce);
            input.Force = GetLatchedInputForce(latchedInputForce);
            var relay = GetRelay(enableForce, latchedInputForce);
            EvalutateForcesWithDelay();
            Assert.Equal(GetLatchedInputForce(latchedInputForce), relay.OutputAxle.GetNetForce());

            enable.Force = Force.ZeroForce;
            EvalutateForcesWithDelay();
            Assert.Equal(GetLatchedInputForce(latchedInputForce), relay.OutputAxle.GetNetForce());
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        public void TestThat_RelayStaysEngagedWhenControlForceIsReversed(int enableForce, int latchedInputForce)
        {
            enable.Force = GetEnableForce(enableForce);
            input.Force = GetLatchedInputForce(latchedInputForce);
            var relay = GetRelay(enableForce, latchedInputForce);
            EvalutateForcesWithDelay();

            enable.Force = GetDisableForce(enableForce);
            EvalutateForcesWithDelay();

            Assert.Equal(GetLatchedInputForce(latchedInputForce), relay.OutputAxle.GetNetForce());
        }

        private Force GetEnableForce(int enableForce)
        {
            return new Force(enableForce);
        }

        private Force GetDisableForce(int enableForce)
        {
            return new Force(enableForce * -1);
        }

        private Force GetLatchedInputForce(int latchedInputForce)
        {
            return new Force(latchedInputForce);
        }

        private Force GetNonLatchedInputForce(int latchedInputForce)
        {
            return new Force(latchedInputForce * -1);
        }

        private Relay GetRelay(int enableForce, int latchedInputForce)
        {
            var couplingService = testUtil.GetSingletonCouplingService();
            var relay = new Relay(
                couplingService: couplingService,
                paddleFactory: testUtil.GetPaddleFactory(),
                enableDirection: enableForce < 0 ? Direction.Negative : Direction.Positive,
                inputDirection: latchedInputForce < 0 ? Direction.Negative : Direction.Positive,
                name: (enableForce < 0 ? "negative" : "positive") + " relay");

            couplingService.CreateNewLockedCoupling(enable, relay.Enable);
            couplingService.CreateNewLockedCoupling(input, relay.InputAxle);

            return relay;
        }

        private void EvalutateForcesWithDelay()
        {
            forceEvaluator.EvaluateForces();
            Thread.Sleep(1000);
            forceEvaluator.EvaluateForces();
        }
    }
}