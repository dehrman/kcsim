using System;
using Xunit;

using KCSim.Parts.Mechanical;
using KCSim.Physics;

namespace KCSimTests
{
    public class DiodeTests
    {
        private Diode positiveDiode;
        private Diode negativeDiode;

        public DiodeTests()
        {
            positiveDiode = new Diode(isPositiveDirection: true);
            negativeDiode = new Diode(isPositiveDirection: false);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void TestThat_PositiveInput_Yields_EquivalentPositiveOutput(int value)
        {
            Axle inputAxle = positiveDiode.InputAxle;
            Axle outputAxle = positiveDiode.OutputAxle;
            inputAxle.AddForce(new Force(value));
            Assert.Equal(value, outputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_MultiplePositiveInputs_Yield_MaximumPositiveOutput()
        {
            Axle inputAxle = positiveDiode.InputAxle;
            Axle outputAxle = positiveDiode.OutputAxle;
            inputAxle.AddForce(new Force(1));
            inputAxle.AddForce(new Force(3));
            inputAxle.AddForce(new Force(2));
            Assert.Equal(3, outputAxle.GetNetForce().Velocity);
        }

        [Fact]
        public void TestThat_RemovingInputForce_Yields_NextHighestPositiveOutput()
        {
            Axle inputAxle = positiveDiode.InputAxle;
            Axle outputAxle = positiveDiode.OutputAxle;
            inputAxle.AddForce(new Force(1));
            Force force3 = new Force(3);
            inputAxle.AddForce(force3);
            inputAxle.AddForce(new Force(2));
            inputAxle.RemoveForce(force3);
            Assert.Equal(2, outputAxle.GetNetForce().Velocity);
        }

        /*
         * 
        private void runDiodeTest()
        {

            Console.WriteLine("Adding negative slow force on input axle");
            inputAxle.RemoveAllForces();
            inputAxle.AddForce(new Force(-1));
            printState();

            Console.WriteLine("Removing all forces on input axle...");
            inputAxle.RemoveAllForces();
            printState();

            Console.WriteLine("Adding positive fast force on output axle...");
            outputAxle.AddForce(doubleForce);
            Console.WriteLine("Adding positive slow force on input axle");
            inputAxle.AddForce(new Force(1));
            printState();

            Console.WriteLine("Removing positive fast force on output axle...");
            outputAxle.RemoveForce(doubleForce);
            printState();
        }
         */
    }
}