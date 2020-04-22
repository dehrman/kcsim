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
        private readonly ICouplingMonitor couplingMonitor;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch inputSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch();
        private readonly BidirectionalLatch bidirectionalLatch;

        public BidirectionalLatchTests()
        {
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();
            couplingService = testUtil.GetSingletonCouplingService();

            bidirectionalLatch = new BidirectionalLatch(
                couplingService: couplingService,
                relayFactory: testUtil.GetMockRelayFactory().Object);

            couplingService.CreateNewLockedCoupling(motor, bidirectionalLatch.Power);
            couplingService.CreateNewLockedCoupling(inputSwitch, bidirectionalLatch.Input);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(-1, 0, -1)]
        [InlineData(0, 0, 0)]
        public void TestThat_ControlValuesArePersistedToOutputAsExpected(int input, int expectedPositiveOutput, int expectedNegativeOutput)
        {
            motor.Force = new Force(1);
            inputSwitch.Force = new Force(input);
            couplingMonitor.EvaluateForces();
            Assert.Equal(new Force(expectedNegativeOutput), bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(new Force(expectedPositiveOutput), bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(-1, 0, -1)]
        [InlineData(0, 0, 0)]
        public void TestThat_ControlValuesRemainLatchedAfterControlGoesToZero(int input, int expectedPositiveOutput, int expectedNegativeOutput)
        {
            motor.Force = new Force(1);

            // Latch in data only briefly, then remove the input. This only works because the mock motion timer
            // invokes its callback functions immediately in test environments. In a real simulation (which
            // is an oxymoron), we would need to wait some time before removing the force on the control axle.
            inputSwitch.Force = new Force(input);
            couplingMonitor.EvaluateForces();
            inputSwitch.Force = Force.ZeroForce;
            couplingMonitor.EvaluateForces();

            Assert.Equal(new Force(expectedNegativeOutput), bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(new Force(expectedPositiveOutput), bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenNoPowerIsProvided_OutputIsZero()
        {
            motor.Force = Force.ZeroForce;
            inputSwitch.Force = new Force(1);
            couplingMonitor.EvaluateForces();
            Assert.Equal(Force.ZeroForce, bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(Force.ZeroForce, bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }
    }
}
