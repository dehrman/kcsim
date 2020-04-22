using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class NotGateTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ICouplingMonitor couplingMonitor;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch inputSwitch = new ExternalSwitch();
        private readonly NotGate notGate;

        public NotGateTests()
        {
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();
            couplingService = testUtil.GetSingletonCouplingService();

            var gateFactory = new GateFactory(couplingService, testUtil.GetMockBidirectionalLatchFactory().Object);
            notGate = gateFactory.CreateNewNotGate();

            couplingService.CreateNewLockedCoupling(inputSwitch, notGate.Input);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        public void TestThat_TruthTableHolds(int input, int expectedOutput)
        {
            inputSwitch.Force = new Force(input);
            couplingMonitor.EvaluateForces();
            Assert.Equal(new Force(expectedOutput), notGate.Output.GetNetForce());
        }
    }
}
