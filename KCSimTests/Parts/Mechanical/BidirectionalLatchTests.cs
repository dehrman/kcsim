using Xunit;

using KCSim.Parts.Mechanical;
using KCSim.Physics;
using Moq;
using KCSim;

namespace KCSimTests
{
    public class BidirectionalLatchTests
    {
        private BidirectionalLatch bidirectionalLatch;

        public BidirectionalLatchTests()
        {
            bidirectionalLatch = new BidirectionalLatch(relayFactory: TestUtil.GetMockRelayFactory().Object);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(-1, 0, -1)]
        [InlineData(0, 0, 0)]
        public void TestThat_ControlValuesArePersistedToOutputAsExpected(int input, int expectedPositiveOutput, int expectedNegativeOutput)
        {
            bidirectionalLatch.Power.AddForce(new Force(1));
            bidirectionalLatch.Input.AddForce(new Force(input));

            Assert.Equal(new Force(expectedNegativeOutput), bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(new Force(expectedPositiveOutput), bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(-1, 0, -1)]
        [InlineData(0, 0, 0)]
        public void TestThat_ControlValuesRemainLatchedAfterControlGoesToZero(int input, int expectedPositiveOutput, int expectedNegativeOutput)
        {
            bidirectionalLatch.Power.AddForce(new Force(1));

            // Latch in data only briefly, then remove the input. This only works because the mock motion timer
            // invokes its callback functions asynchronously in test environments. In a real simulation (which
            // is an oxymoron), we would need to wait some time before removing the force on the control axle.
            bidirectionalLatch.Input.AddForce(new Force(input));
            bidirectionalLatch.Input.RemoveAllForces();

            Assert.Equal(new Force(expectedNegativeOutput), bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(new Force(expectedPositiveOutput), bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenNoPowerIsProvided_OutputIsZero()
        {
            bidirectionalLatch.Power.AddForce(new Force(1));
            bidirectionalLatch.Input.AddForce(new Force(1));
            bidirectionalLatch.Power.RemoveAllForces();

            Assert.Equal(Force.ZeroForce, bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(Force.ZeroForce, bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }
    }
}
