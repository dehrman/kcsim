using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class XorGateTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ICouplingMonitor couplingMonitor;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch inputASwitch = new ExternalSwitch();
        private readonly ExternalSwitch inputBSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch();
        private readonly XorGate xorGate;

        public XorGateTests()
        {
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();
            couplingService = testUtil.GetSingletonCouplingService();

            IGateFactory gateFactory = new GateFactory(couplingService, testUtil.GetMockBidirectionalLatchFactory().Object);
            xorGate = gateFactory.CreateNewXorGate();

            couplingService.CreateNewLockedCoupling(inputASwitch, xorGate.InputA);
            couplingService.CreateNewLockedCoupling(inputBSwitch, xorGate.InputB);
            couplingService.CreateNewLockedCoupling(motor, xorGate.Power);

            motor.Force = new Force(1);
        }

        [Theory]
        [InlineData(-1, -1, -1)]
        [InlineData(-1, 1, 1)]
        [InlineData(1, -1, 1)]
        [InlineData(1, 1, -1)]
        public void TestThat_TruthTableHolds(int inputA, int inputB, int expectedOutput)
        {
            inputASwitch.Force = new Force(inputA);
            inputBSwitch.Force = new Force(inputB);
            couplingMonitor.EvaluateForces();
            Assert.Equal(new Force(expectedOutput), xorGate.Output.GetNetForce());
        }
    }
}
