using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using Xunit;

namespace KCSimTests.Parts.Logical
{
    public class BufferTests
    {
        private readonly TestUtil testUtil = new TestUtil();
        private readonly ICouplingMonitor couplingMonitor;
        private readonly ICouplingService couplingService;

        private readonly ExternalSwitch inputSwitch = new ExternalSwitch();
        private readonly ExternalSwitch motor = new ExternalSwitch();

        private readonly Buffer buffer;

        public BufferTests()
        {
            couplingMonitor = testUtil.GetSingletonCouplingMonitor();
            couplingService = testUtil.GetSingletonCouplingService();

            IGateFactory gateFactory = testUtil.GetGateFactory();
            buffer = gateFactory.CreateNewBuffer();

            couplingService.CreateNewLockedCoupling(inputSwitch, buffer.Input);
            couplingService.CreateNewLockedCoupling(motor, buffer.Power);
        }
        
        [Theory]
        [InlineData(-1, 2, -2)]
        [InlineData(1, 2, 2)]
        [InlineData(2, 1, 1)]
        public void TestThat_OutputPowerIsAtPowerLevelRatherThanInputLevel(int input, int power, int expectedOutput)
        {
            inputSwitch.Force = new Force(input);
            motor.Force = new Force(power);
            couplingMonitor.EvaluateForces();
            Assert.Equal(new Force(expectedOutput), buffer.Output.GetNetForce());
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public void TestThat_NonPositivePowerYieldsHighImpedanceOutput(int input, int power)
        {
            inputSwitch.Force = new Force(input);
            motor.Force = new Force(power);
            couplingMonitor.EvaluateForces();
            Assert.Equal(Force.ZeroForce, buffer.Output.GetNetForce());
        }
    }
}
