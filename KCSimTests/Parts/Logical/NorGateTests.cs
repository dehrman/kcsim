using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class NorGateTests : BaseGateTest
    {
        private readonly ExternalSwitch inputASwitch = new ExternalSwitch();
        private readonly ExternalSwitch inputBSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch(new Force(1));
        private readonly NorGate norGate;

        public NorGateTests()
        {
            norGate = gateFactory.CreateNewNorGate();

            couplingService.CreateNewLockedCoupling(inputASwitch, norGate.InputA);
            couplingService.CreateNewLockedCoupling(inputBSwitch, norGate.InputB);
            couplingService.CreateNewLockedCoupling(motor, norGate.Power);
        }

        [Theory]
        [InlineData(-1, -1, 1)]
        [InlineData(-1, 1, -1)]
        [InlineData(1, -1, -1)]
        [InlineData(1, 1, -1)]
        public void TestThat_TruthTableHolds(int inputA, int inputB, int expectedOutput)
        {
            inputASwitch.Force = new Force(inputA * 1000);
            inputBSwitch.Force = new Force(inputB * 1000);
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), norGate.Output.GetNetForce());
        }
    }
}
