using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class NandGateTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ForceEvaluator forceEvaluator;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch inputASwitch = new ExternalSwitch();
        private readonly ExternalSwitch inputBSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch(new Force(1));
        private readonly NandGate nandGate;

        public NandGateTests()
        {
            forceEvaluator = testUtil.GetSingletonForceEvaluator();
            couplingService = testUtil.GetSingletonCouplingService();

            IGateFactory gateFactory = testUtil.GetGateFactory();
            nandGate = gateFactory.CreateNewNandGate();

            couplingService.CreateNewLockedCoupling(inputASwitch, nandGate.InputA);
            couplingService.CreateNewLockedCoupling(inputBSwitch, nandGate.InputB);
            couplingService.CreateNewLockedCoupling(motor, nandGate.Power);
        }

        [Theory]
        [InlineData(-1, -1, 1)]
        [InlineData(-1, 1, 1)]
        [InlineData(1, -1, 1)]
        [InlineData(1, 1, -1)]
        public void TestThat_TruthTableHolds(int inputA, int inputB, int expectedOutput)
        {
            inputASwitch.Force = new Force(inputA);
            inputBSwitch.Force = new Force(inputB);
            forceEvaluator.EvaluateForces();
            Assert.Equal(new Force(expectedOutput), nandGate.Output.GetNetForce());
        }
    }
}
