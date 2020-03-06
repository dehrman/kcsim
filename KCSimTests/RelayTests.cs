using System;
using Xunit;

using KCSim.Parts.Mechanical;
using KCSim.Physics;

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
            relay = new Relay(isControlPositiveDirection: true, isInputPositiveDirection: true);
        }

        [Fact]
        public void TestThat_WhenNoForceIsOnControl_OutputStaysZero()
        {
            Axle inputAxle = relay.InputAxle;
            Axle outputAxle = relay.OutputAxle;
            Axle controlAxle = relay.ControlAxle;

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

            Arm arm = relay.

            Assert.Equal(0, outputAxle.GetNetForce().Velocity);
        }
    }
}