using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class OrGateTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ICouplingMonitor couplingMonitor;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch inputASwitch = new ExternalSwitch();
        private readonly ExternalSwitch inputBSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch(new Force(1));
        private readonly OrGate orGate;

        public OrGateTests()
        {
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();
            couplingService = testUtil.GetSingletonCouplingService();

            var gateFactory = testUtil.GetGateFactory();
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
            inputASwitch.Force = new Force(inputA);
            inputBSwitch.Force = new Force(inputB);
            couplingMonitor.EvaluateForces();
            Assert.Equal(new Force(expectedOutput), orGate.Output.GetNetForce());
        }
    }
}
