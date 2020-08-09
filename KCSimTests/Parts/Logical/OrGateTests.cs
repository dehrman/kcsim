using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class OrGateTests : BaseGateTest
    {
        private readonly ExternalSwitch inputASwitch = new ExternalSwitch();
        private readonly ExternalSwitch inputBSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch(new Force(1));
        private readonly OrGate orGate;

        public OrGateTests()
        {
            orGate = gateFactory.CreateNewOrGate();

            couplingService.CreateNewLockedCoupling(inputASwitch, orGate.InputA);
            couplingService.CreateNewLockedCoupling(inputBSwitch, orGate.InputB);
            couplingService.CreateNewLockedCoupling(motor, orGate.Power);
        }

        [Theory]
        [InlineData(-1, -1, -1)]
        [InlineData(-1, 1, 1)]
        [InlineData(1, -1, 1)]
        [InlineData(1, 1, 1)]
        public void TestThat_TruthTableHolds(int inputA, int inputB, int expectedOutput)
        {
            inputASwitch.Force = new Force(inputA * 1000);
            inputBSwitch.Force = new Force(inputB * 1000);
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), orGate.Output.GetNetForce());
        }
    }
}
