using KCSim.Parts.Mechanical;
using KCSim.Physics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KCSimTests
{
    [TestClass]
    public class DiodeTests
    {
        private Diode positiveDiode;
        private Diode negativeDiode;

        [TestInitialize]
        public void Initialize()
        {
            positiveDiode = new Diode(isPositiveDirection: true);
            negativeDiode = new Diode(isPositiveDirection: false);
        }

        [TestMethod]
        public void TestThat_PositiveInput_Yields_EquivalentPositiveOutput()
        {
            Axle inputAxle = positiveDiode.InputAxle;
            Axle outputAxle = positiveDiode.OutputAxle;
            inputAxle.AddForce(new Force(1));
            Assert.AreEqual(1, outputAxle.GetNetForce().Velocity);
        }

        [TestMethod]
        public void TestThat_MultiplePositiveInputs_Yield_MaximumPositiveOutput()
        {
            Axle inputAxle = positiveDiode.InputAxle;
            Axle outputAxle = positiveDiode.OutputAxle;
            inputAxle.AddForce(new Force(1));
            inputAxle.AddForce(new Force(3));
            inputAxle.AddForce(new Force(2));
            Assert.AreEqual(3, outputAxle.GetNetForce().Velocity);
        }

        [TestMethod]
        public void TestThat_RemovingInputForce_Yields_NextHighestPositiveOutput()
        {
            Axle inputAxle = positiveDiode.InputAxle;
            Axle outputAxle = positiveDiode.OutputAxle;
            inputAxle.AddForce(new Force(1));
            Force force3 = new Force(3);
            inputAxle.AddForce(force3);
            inputAxle.AddForce(new Force(2));
            inputAxle.RemoveForce(force3);
            Assert.AreEqual(2, outputAxle.GetNetForce().Velocity);
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
