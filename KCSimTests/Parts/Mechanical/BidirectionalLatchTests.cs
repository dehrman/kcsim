using Xunit;

using KCSim.Parts.Mechanical;
using KCSim.Physics;
using Moq;
using KCSim;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics.Couplings;

namespace KCSimTests
{
    public class BidirectionalLatchTests
    {
        private TestUtil testUtil = new TestUtil();

        private readonly Coupling inputCoupling;
        private readonly Coupling motorCoupling;
        private readonly BidirectionalLatch bidirectionalLatch;

        public BidirectionalLatchTests()
        {
            ICouplingService couplingService = testUtil.GetSingletonCouplingService();
            bidirectionalLatch = new BidirectionalLatch(
                couplingService: couplingService,
                relayFactory: testUtil.GetMockRelayFactory().Object);

            var input = new HumanSwitch();
            var motor = new Motor();
            inputCoupling = couplingService.CreateNewLockedCoupling(input, bidirectionalLatch.Input);
            motorCoupling = couplingService.CreateNewLockedCoupling(motor, bidirectionalLatch.Power);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(-1, 0, -1)]
        [InlineData(0, 0, 0)]
        public void TestThat_ControlValuesArePersistedToOutputAsExpected(int input, int expectedPositiveOutput, int expectedNegativeOutput)
        {
            bidirectionalLatch.Power.UpdateForce(motorCoupling, new Force(1));
            bidirectionalLatch.Input.UpdateForce(inputCoupling, new Force(input));

            Assert.Equal(new Force(expectedNegativeOutput), bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(new Force(expectedPositiveOutput), bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(-1, 0, -1)]
        [InlineData(0, 0, 0)]
        public void TestThat_ControlValuesRemainLatchedAfterControlGoesToZero(int input, int expectedPositiveOutput, int expectedNegativeOutput)
        {
            bidirectionalLatch.Power.UpdateForce(motorCoupling, new Force(1));

            // Latch in data only briefly, then remove the input. This only works because the mock motion timer
            // invokes its callback functions asynchronously in test environments. In a real simulation (which
            // is an oxymoron), we would need to wait some time before removing the force on the control axle.
            bidirectionalLatch.Input.UpdateForce(inputCoupling, new Force(input));
            bidirectionalLatch.Input.RemoveAllForces();

            Assert.Equal(new Force(expectedNegativeOutput), bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(new Force(expectedPositiveOutput), bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenNoPowerIsProvided_OutputIsZero()
        {
            bidirectionalLatch.Power.UpdateForce(motorCoupling, Force.ZeroForce);
            bidirectionalLatch.Input.UpdateForce(inputCoupling, new Force(1));

            Assert.Equal(Force.ZeroForce, bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(Force.ZeroForce, bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }
    }
}
