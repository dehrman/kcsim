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
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ForceEvaluator forceEvaluator;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch inputSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch();
        private readonly BidirectionalLatch bidirectionalLatch;

        public BidirectionalLatchTests()
        {
            forceEvaluator = testUtil.GetSingletonForceEvaluator();
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
            forceEvaluator.EvaluateForces();
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
            forceEvaluator.EvaluateForces();
            inputSwitch.Force = Force.ZeroForce;
            forceEvaluator.EvaluateForces();

            Assert.Equal(new Force(expectedNegativeOutput), bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(new Force(expectedPositiveOutput), bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }

        [Fact]
        public void TestThat_WhenNoPowerIsProvided_OutputIsZero()
        {
            motor.Force = Force.ZeroForce;
            inputSwitch.Force = new Force(1);
            forceEvaluator.EvaluateForces();
            Assert.Equal(Force.ZeroForce, bidirectionalLatch.OutputAxleNegative.GetNetForce());
            Assert.Equal(Force.ZeroForce, bidirectionalLatch.OutputAxlePositive.GetNetForce());
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public void TestThat_NewValuesAreLatchedIn(int oldValue, int newValue)
        {
            motor.Force = new Force(1);

            // Latch in the initial value.
            inputSwitch.Force = new Force(oldValue);
            forceEvaluator.EvaluateForces();

            // Latch in the new value.
            inputSwitch.Force = new Force(newValue);
            forceEvaluator.EvaluateForces();

            if (newValue < 0)
            {
                Assert.Equal(new Force(newValue), bidirectionalLatch.OutputAxleNegative.GetNetForce());
                Assert.Equal(new Force(0), bidirectionalLatch.OutputAxlePositive.GetNetForce());
            }
            else if (newValue < 0)
            {
                Assert.Equal(new Force(0), bidirectionalLatch.OutputAxleNegative.GetNetForce());
                Assert.Equal(new Force(newValue), bidirectionalLatch.OutputAxlePositive.GetNetForce());
            }

            // And now back to old values.
            inputSwitch.Force = new Force(oldValue);
            forceEvaluator.EvaluateForces();

            if (oldValue < 0)
            {
                Assert.Equal(new Force(oldValue), bidirectionalLatch.OutputAxleNegative.GetNetForce());
                Assert.Equal(new Force(0), bidirectionalLatch.OutputAxlePositive.GetNetForce());
            }
            else if (oldValue < 0)
            {
                Assert.Equal(new Force(0), bidirectionalLatch.OutputAxleNegative.GetNetForce());
                Assert.Equal(new Force(newValue), bidirectionalLatch.OutputAxlePositive.GetNetForce());
            }

        }
    }
}
