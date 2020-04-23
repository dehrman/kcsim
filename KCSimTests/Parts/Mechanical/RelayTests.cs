using KCSim;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests
{
    public class RelayTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private ICouplingMonitor couplingMonitor;
        private ICouplingService couplingService;

        private readonly ExternalSwitch enable = new ExternalSwitch(name: "enable switch");
        private readonly ExternalSwitch input = new ExternalSwitch(name: "input switch");

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
            couplingMonitor.EvaluateForces();
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
            couplingMonitor.EvaluateForces();
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
            couplingMonitor.EvaluateForces();
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
            couplingMonitor.EvaluateForces();
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
            couplingMonitor.EvaluateForces();
            Assert.Equal(GetLatchedInputForce(latchedInputForce), relay.OutputAxle.GetNetForce());

            enable.Force = Force.ZeroForce;
            couplingMonitor.EvaluateForces();
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
            couplingMonitor.EvaluateForces();

            enable.Force = GetDisableForce(enableForce);
            couplingMonitor.EvaluateForces();

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
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();
            couplingService = testUtil.GetSingletonCouplingService();
            var relay = new Relay(
                couplingService: couplingService,
                paddleFactory: testUtil.GetMockPaddleFactory().Object,
                enableDirection: enableForce < 0 ? Direction.Negative : Direction.Positive,
                inputDirection: latchedInputForce < 0 ? Direction.Negative : Direction.Positive,
                name: "relay");

            couplingService.CreateNewLockedCoupling(enable, relay.Enable);
            couplingService.CreateNewLockedCoupling(input, relay.InputAxle);

            return relay;
        }
    }
}