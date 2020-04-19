using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class AndGateTests
    {
        private TestUtil testUtil = new TestUtil();

        private readonly Coupling inputACoupling;
        private readonly Coupling inputBCoupling;
        private readonly AndGate andGate;

        public AndGateTests()
        {
            ICouplingService couplingService = testUtil.GetSingletonCouplingService();
            andGate = new AndGate(couplingService, testUtil.GetMockBidirectionalLatchFactory().Object);

            HumanSwitch inputA = new HumanSwitch();
            HumanSwitch inputB = new HumanSwitch();
            Motor motor = new Motor();

            inputACoupling = couplingService.CreateNewLockedCoupling(inputA, andGate.InputA);
            inputBCoupling = couplingService.CreateNewLockedCoupling(inputB, andGate.InputB);
            Coupling motorCoupling = couplingService.CreateNewLockedCoupling(motor, andGate.Power);

            andGate.Power.UpdateForce(motorCoupling, new Force(1));
        }

        [Theory]
        [InlineData(-1, -1, -1)]
        [InlineData(-1, 1, -1)]
        [InlineData(1, -1, -1)]
        [InlineData(1, 1, 1)]
        public void TestThat_TruthTableHolds(int inputA, int inputB, int expectedOutput)
        {
            andGate.InputA.UpdateForce(inputACoupling, new Force(inputA));
            Assert.Equal(Force.ZeroForce, andGate.InputB.GetNetForce());
            andGate.InputB.UpdateForce(inputBCoupling, new Force(inputB));
            Assert.Equal(new Force(expectedOutput), andGate.Output.GetNetForce());
        }
    }
}
