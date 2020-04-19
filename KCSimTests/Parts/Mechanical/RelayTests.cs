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
        private TestUtil testUtil = new TestUtil();

        private readonly Coupling enableCoupling;
        private readonly Coupling inputCoupling;
        private Relay relay;

        public RelayTests()
        {
            ICouplingService couplingService = testUtil.GetSingletonCouplingService();
            relay = new Relay(
                couplingService: couplingService,
                paddleFactory: testUtil.GetMockPaddleFactory().Object,
                enableDirection: Direction.Positive,
                inputDirection: Direction.Positive);

            var enable = new HumanSwitch();
            var input = new HumanSwitch();
            enableCoupling = couplingService.CreateNewLockedCoupling(enable, relay.Enable);
            inputCoupling = couplingService.CreateNewLockedCoupling(input, relay.InputAxle);
        }

        [Fact]
        public void TestThat_WhenNoForceIsOnControl_OutputStaysZero()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;

            inputAxle.UpdateForce(inputCoupling, new Force(1));
            Assert.Equal(Force.ZeroForce, outputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenPositiveControl_OutputIsPowered()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.Enable;

            inputAxle.UpdateForce(inputCoupling, new Force(1));
            controlAxle.UpdateForce(enableCoupling, new Force(1));

            Assert.Equal(new Force(1), outputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenNegativeControl_OutputIsNotPowered()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.Enable;

            inputAxle.UpdateForce(inputCoupling, new Force(1));
            controlAxle.UpdateForce(enableCoupling, new Force(-1));

            Assert.Equal(Force.ZeroForce, outputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenRelayIsEngaged_ButNegativeInputIsProvided_OutputIsNotPowered()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.Enable;

            inputAxle.UpdateForce(inputCoupling, new Force(-1));
            controlAxle.UpdateForce(enableCoupling, new Force(1));

            Assert.Equal(Force.ZeroForce, outputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_RelayStaysEngagedWhenControlForceIsRemoved()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.Enable;

            inputAxle.UpdateForce(inputCoupling, new Force(1));
            
            // Start with the relay engaged.
            controlAxle.UpdateForce(enableCoupling, new Force(1));
            Assert.Equal(new Force(1), outputAxle.GetNetForce());

            // Now remove the control force.
            controlAxle.RemoveAllForces();

            Assert.Equal(new Force(1), outputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_RelayDisengagesWhenControlForceIsReversed()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.Enable;

            inputAxle.UpdateForce(inputCoupling, new Force(1));

            // Start with the relay engaged.
            controlAxle.UpdateForce(enableCoupling, new Force(1));
            Assert.Equal(new Force(1), outputAxle.GetNetForce());

            // Now disengage the relay.
            controlAxle.RemoveAllForces();
            controlAxle.UpdateForce(enableCoupling, new Force(-1));

            Assert.Equal(Force.ZeroForce, outputAxle.GetNetForce());
        }
    }
}