using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class NotGateTests : BaseGateTest
    {
        private readonly ExternalSwitch inputSwitch = new ExternalSwitch();
        private readonly NotGate notGate;

        public NotGateTests()
        {
            notGate = gateFactory.CreateNewNotGate();

            couplingService.CreateNewLockedCoupling(inputSwitch, notGate.Input);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        public void TestThat_TruthTableHolds(int input, int expectedOutput)
        {
            inputSwitch.Force = new Force(input * 1000);
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), notGate.Output.GetNetForce());
        }
    }
}
