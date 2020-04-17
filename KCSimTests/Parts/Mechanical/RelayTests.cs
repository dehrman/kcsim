using Xunit;

using KCSim.Parts.Mechanical;
using KCSim.Physics;
using Moq;
using KCSim;

namespace KCSimTests
{
    /**
     * Need to update this to play nice with the new mode of class design, using dependency injection.
     * But for now I'm tired and will wrap up for the night.
     */
    public class RelayTests
    {
        private Relay relay;

        public RelayTests()
        {
            relay = new Relay(
                paddleFactory: TestUtil.GetMockPaddleFactory().Object,
                isControlPositiveDirection: true,
                isInputPositiveDirection: true);
        }

        [Fact]
        public void TestThat_WhenNoForceIsOnControl_OutputStaysZero()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;

            inputAxle.AddForce(new Force(1));
            Assert.Equal(Force.ZeroForce, outputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenPositiveControl_OutputIsPowered()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.Enable;

            inputAxle.AddForce(new Force(1));
            controlAxle.AddForce(new Force(1));

            Assert.Equal(new Force(1), outputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenNegativeControl_OutputIsNotPowered()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.Enable;

            inputAxle.AddForce(new Force(1));
            controlAxle.AddForce(new Force(-1));

            Assert.Equal(Force.ZeroForce, outputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenRelayIsEngaged_ButNegativeInputIsProvided_OutputIsNotPowered()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.Enable;

            inputAxle.AddForce(new Force(-1));
            controlAxle.AddForce(new Force(1));

            Assert.Equal(Force.ZeroForce, outputAxle.GetNetForce());
        }

        [Fact]
        public void TestThat_RelayStaysEngagedWhenControlForceIsRemoved()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.Enable;

            inputAxle.AddForce(new Force(1));
            
            // Start with the relay engaged.
            controlAxle.AddForce(new Force(1));
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

            inputAxle.AddForce(new Force(1));

            // Start with the relay engaged.
            controlAxle.AddForce(new Force(1));
            Assert.Equal(new Force(1), outputAxle.GetNetForce());

            // Now disengage the relay.
            controlAxle.RemoveAllForces();
            controlAxle.AddForce(new Force(-1));

            Assert.Equal(Force.ZeroForce, outputAxle.GetNetForce());
        }
    }
}