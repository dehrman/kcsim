using System;
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
        private Mock<IPaddleFactory> mockPaddleFactory;
        private Mock<IMotionTimer> mockMotionTimer;

        public RelayTests()
        {
            SetupMocks();
            relay = new Relay(paddleFactory: mockPaddleFactory.Object, isControlPositiveDirection: true, isInputPositiveDirection: true);
        }

        [Fact]
        public void TestThat_WhenNoForceIsOnControl_OutputStaysZero()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;

            inputAxle.AddForce(new Force(1));
            Assert.Equal(0, outputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_WhenPositiveControl_OutputIsPowered()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.ControlAxle;

            inputAxle.AddForce(new Force(1));
            controlAxle.AddForce(new Force(1));

            Assert.Equal(1, outputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_WhenNegativeControl_OutputIsNotPowered()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.ControlAxle;

            inputAxle.AddForce(new Force(1));
            controlAxle.AddForce(new Force(-1));

            Assert.Equal(0, outputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_WhenRelayIsEngaged_ButNegativeInputIsProvided_OutputIsNotPowered()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.ControlAxle;

            inputAxle.AddForce(new Force(-1));
            controlAxle.AddForce(new Force(1));

            Assert.Equal(0, outputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_RelayStaysEngagedWhenControlForceIsRemoved()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.ControlAxle;

            inputAxle.AddForce(new Force(1));
            
            // Start with the relay engaged.
            controlAxle.AddForce(new Force(1));
            Assert.Equal(1, outputAxle.GetNetForce().Velocity);

            // Now remove the control force.
            controlAxle.RemoveAllForces();

            Assert.Equal(1, outputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_RelayDisengagesWhenControlForceIsReversed()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.ControlAxle;

            inputAxle.AddForce(new Force(1));

            // Start with the relay engaged.
            controlAxle.AddForce(new Force(1));
            Assert.Equal(1, outputAxle.GetNetForce().Velocity);

            // Now disengage the relay.
            controlAxle.RemoveAllForces();
            controlAxle.AddForce(new Force(-1));

            Assert.Equal(0, outputAxle.GetNetForce().Velocity);
        }

        private void SetupMocks()
        {
            mockPaddleFactory = new Mock<IPaddleFactory>();
            mockMotionTimer = new Mock<IMotionTimer>();

            // Set up the motion timer such that the delegate is invoked as soon as the timer is started.
            mockMotionTimer.Setup(x => x.Start(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<IMotionTimer.OnTimerCompletionDelegate>()))
                .Callback<double, double, IMotionTimer.OnTimerCompletionDelegate>(
                    (d, v, onTimerCompletionDelegate) => onTimerCompletionDelegate.Invoke(v));

            Paddle paddle = new Paddle(mockMotionTimer.Object);
            mockPaddleFactory.Setup(x => x.CreateNew(It.IsAny<Paddle.Position>(), It.IsAny<string>())).Returns(paddle);
        }
    }
}