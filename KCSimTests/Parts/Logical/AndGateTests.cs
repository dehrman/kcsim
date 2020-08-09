using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    /// <summary>
    /// The and gate tests.
    /// </summary>
    public class AndGateTests : BaseGateTest
    {
        private readonly ExternalSwitch inputASwitch = new ExternalSwitch();
        private readonly ExternalSwitch inputBSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch(new Force(1 * 1000));
        private readonly AndGate andGate;

        public AndGateTests()
        {
            andGate = gateFactory.CreateNewAndGate();

            couplingService.CreateNewLockedCoupling(inputASwitch, andGate.InputA);
            couplingService.CreateNewLockedCoupling(inputBSwitch, andGate.InputB);
            couplingService.CreateNewLockedCoupling(motor, andGate.Power);
        }

        [Theory]
        [InlineData(-1, -1, -1)]
        [InlineData(-1, 1, -1)]
        [InlineData(1, -1, -1)]
        [InlineData(1, 1, 1)]
        public void TestThat_TruthTableHolds(int inputA, int inputB, int expectedOutput)
        {
            inputASwitch.Force = new Force(inputA * 1000);
            inputBSwitch.Force = new Force(inputB * 1000);
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), andGate.Output.GetNetForce());
        }

        [Fact]
        public void TestThat_ValuesCanToggleBackAndForth()
        {
            inputASwitch.Force = new Force(1 * 1000);

            inputBSwitch.Force = new Force(-1 * 1000);
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(-1 * 1000), andGate.Output.GetNetForce());

            inputBSwitch.Force = new Force(1 * 1000);
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(1 * 1000), andGate.Output.GetNetForce());

            inputBSwitch.Force = new Force(-1 * 1000);
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(-1 * 1000), andGate.Output.GetNetForce());
        }

        [Theory]
        [InlineData(-1, -1, -1)]
        [InlineData(-1, 1, -1)]
        [InlineData(1, -1, -1)]
        [InlineData(1, 1, 1)]
        public void TestThat_InputsAreUnpowered_OutputsRemainTheSame(int inputA, int inputB, int expectedOutput)
        {
            inputASwitch.Force = new Force(inputA * 1000);
            inputBSwitch.Force = new Force(inputB * 1000);
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), andGate.Output.GetNetForce());

            inputBSwitch.Force = Force.ZeroForce;
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), andGate.Output.GetNetForce());

            inputASwitch.Force = Force.ZeroForce;
            EvaluateForcesWithDelay();
            TestUtil.AssertDirectionsEqual(new Force(expectedOutput), andGate.Output.GetNetForce());
        }
    }
}
