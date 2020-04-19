using System;
using Xunit;

using KCSim.Parts.Mechanical;
using KCSim.Physics;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics.Couplings;
using KCSim;
using KCSim.Parts.Mechanical.Machines;

namespace KCSimTests
{
    public class DiodeTests
    {
        private TestUtil testUtil = new TestUtil();

        private readonly Coupling inputCoupling;
        private readonly Diode positiveDiode;
        private readonly Diode negativeDiode;

        public DiodeTests()
        {
            ICouplingService couplingService = testUtil.GetSingletonCouplingService();
            positiveDiode = new Diode(couplingService, Direction.Positive);
            negativeDiode = new Diode(couplingService, Direction.Negative);

            var input = new HumanSwitch();
            inputCoupling = couplingService.CreateNewLockedCoupling(input, positiveDiode.InputAxle);
            inputCoupling = couplingService.CreateNewLockedCoupling(input, negativeDiode.InputAxle);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void TestThat_PositiveInput_Yields_EquivalentPositiveOutput(int value)
        {
            Axle inputAxle = positiveDiode.InputAxle;
            Axle outputAxle = positiveDiode.OutputAxle;
            inputAxle.UpdateForce(inputCoupling, new Force(value));
            Assert.Equal(value, outputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_MultiplePositiveInputs_Yield_MaximumPositiveOutput()
        {
            Axle inputAxle = positiveDiode.InputAxle;
            Axle outputAxle = positiveDiode.OutputAxle;
            inputAxle.UpdateForce(inputCoupling, new Force(1));
            inputAxle.UpdateForce(inputCoupling, new Force(3));
            inputAxle.UpdateForce(inputCoupling, new Force(2));
            Assert.Equal(3, outputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_RemovingInputForce_Yields_NextHighestPositiveOutput()
        {
            Axle inputAxle = positiveDiode.InputAxle;
            Axle outputAxle = positiveDiode.OutputAxle;
            inputAxle.UpdateForce(inputCoupling, new Force(1));
            Force force3 = new Force(3);
            inputAxle.UpdateForce(inputCoupling, force3);
            inputAxle.UpdateForce(inputCoupling, new Force(2));
            inputAxle.UpdateForce(inputCoupling, Force.ZeroForce);
            Assert.Equal(2, outputAxle.GetNetForce().Velocity);
        }

        /*
         * 
        private void runDiodeTest()
        {

            Console.WriteLine("Adding negative slow force on input axle");
            inputAxle.RemoveAllForces();
            inputAxle.UpdateForce(inputCoupling, new Force(-1));
            printState();

            Console.WriteLine("Removing all forces on input axle...");
            inputAxle.RemoveAllForces();
            printState();

            Console.WriteLine("Adding positive fast force on output axle...");
            outputAxle.UpdateForce(inputCoupling, doubleForce);
            Console.WriteLine("Adding positive slow force on input axle");
            inputAxle.UpdateForce(inputCoupling, new Force(1));
            printState();

            Console.WriteLine("Removing positive fast force on output axle...");
            outputAxle.RemoveForce(doubleForce);
            printState();
        }
         */
    }
}