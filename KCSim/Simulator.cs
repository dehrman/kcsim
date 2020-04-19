using System;
using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim
{
    public class Simulator
    {
        Diode diode;
        Axle inputAxle;
        Axle outputAxle;

        public Simulator()
        {
            /*
            diode = new Diode(isPositiveDirection: true);

            inputAxle = diode.InputAxle;
            outputAxle = diode.OutputAxle;
            */
        }

        public void runDiodeTest()
        {
            Console.WriteLine("Running diode test...");
            printState();
            /*
            Console.WriteLine("Adding positive slow force on input axle...");
            inputAxle.AddForce(new Force(1));
            printState();

            Console.WriteLine("Adding positive fast force on input axle...");
            Force doubleForce = new Force(2);
            inputAxle.AddForce(doubleForce);
            printState();

            Console.WriteLine("Removing positive fast force on input axle...");
            inputAxle.RemoveForce(doubleForce);
            printState();

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
            */
        }

        private void printState()
        {
            Console.WriteLine("Condition: Input={" + inputAxle + "}; Output={" + outputAxle + "}");
        }
    }
}
