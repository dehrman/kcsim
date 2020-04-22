using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class NorGateTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ICouplingMonitor couplingMonitor;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch inputASwitch = new ExternalSwitch();
        private readonly ExternalSwitch inputBSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch();
        private readonly NorGate norGate;

        public NorGateTests()
        {
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();
            couplingService = testUtil.GetSingletonCouplingService();

            var gateFactory = new GateFactory(couplingService, testUtil.GetMockBidirectionalLatchFactory().Object);
            norGate = gateFactory.CreateNewNorGate();

            couplingService.CreateNewLockedCoupling(inputASwitch, norGate.InputA);
            couplingService.CreateNewLockedCoupling(inputBSwitch, norGate.InputB);
            couplingService.CreateNewLockedCoupling(motor, norGate.Power);

            motor.Force = new Force(1);
        }

        [Theory]
        [InlineData(-1, -1, 1)]
        [InlineData(-1, 1, -1)]
        [InlineData(1, -1, -1)]
        [InlineData(1, 1, -1)]
        public void TestThat_TruthTableHolds(int inputA, int inputB, int expectedOutput)
        {
            inputASwitch.Force = new Force(inputA);
            inputBSwitch.Force = new Force(inputB);
            couplingMonitor.EvaluateForces();
            Assert.Equal(new Force(expectedOutput), norGate.Output.GetNetForce());
        }
    }
}
