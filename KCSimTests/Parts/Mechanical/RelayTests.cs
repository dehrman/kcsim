using Xunit;
using KCSim.Physics;
using Moq;
using KCSim;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics.Couplings;

namespace KCSimTests
{
    /**
     * Need to update this to play nice with the new mode of class design, using dependency injection.
     * But for now I'm tired and will wrap up for the night.
     */
    public class RelayTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ICouplingMonitor couplingMonitor;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch enable = new ExternalSwitch(name: "enable switch");
        private readonly ExternalSwitch input = new ExternalSwitch(name: "input switch");
        private Relay relay;

        public RelayTests()
        {
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();
            couplingService = testUtil.GetSingletonCouplingService();
            relay = new Relay(
                couplingService: couplingService,
                paddleFactory: testUtil.GetMockPaddleFactory().Object,
                enableDirection: Direction.Positive,
                inputDirection: Direction.Positive,
                name: "relay");

            couplingService.CreateNewLockedCoupling(enable, relay.Enable);
            couplingService.CreateNewLockedCoupling(input, relay.InputAxle);
        }

        [Fact]
        public void TestThat_WhenNoForceIsOnControl_OutputStaysZero()
        {
            enable.Force = Force.ZeroForce;
            input.Force = new Force(1);
            couplingMonitor.EvaluateForces();
            Assert.Equal(Force.ZeroForce, relay.OutputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenPositiveControl_OutputIsPowered()
        {
            enable.Force = new Force(1);
            input.Force = new Force(1);
            couplingMonitor.EvaluateForces();
            Assert.Equal(new Force(1), relay.OutputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenNegativeControl_OutputIsNotPowered()
        {
            enable.Force = new Force(-1);
            input.Force = new Force(1);
            couplingMonitor.EvaluateForces();
            Assert.Equal(Force.ZeroForce, relay.OutputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenRelayIsEngaged_ButNegativeInputIsProvided_OutputIsNotPowered()
        {
            enable.Force = new Force(1);
            input.Force = new Force(-1);
            couplingMonitor.EvaluateForces();
            Assert.Equal(Force.ZeroForce, relay.OutputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_RelayStaysEngagedWhenControlForceIsRemoved()
        {
            enable.Force = new Force(1);
            input.Force = new Force(1);
            couplingMonitor.EvaluateForces();

            enable.Force = Force.ZeroForce;
            couplingMonitor.EvaluateForces();

            Assert.Equal(new Force(1), relay.OutputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_RelayStaysEngagedWhenControlForceIsReversed()
        {
            enable.Force = new Force(1);
            input.Force = new Force(1);
            couplingMonitor.EvaluateForces();

            enable.Force = new Force(-1);
            couplingMonitor.EvaluateForces();

            Assert.Equal(new Force(1), relay.OutputAxle.GetNetForce());
        }
    }
}