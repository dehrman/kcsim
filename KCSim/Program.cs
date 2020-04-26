using System;
using KCSim.Parts.Mechanical;
using KCSim.Physics;
using SimpleInjector;

namespace KCSim
{
    static class Program
    {
        private static readonly Container container;

        static void Main(string[] args)
        {
            Simulator simulator = container.GetInstance<Simulator>();

            simulator.runDiodeTest();
        }

        static Program()
        {
            // 1. Create a new Simple Injector container
            container = new Container();

            // 2. Configure the container (register)
            container.Register<IMotionTimer, MotionTimer>();
            container.Register<ForceEvaluator>();
            container.Register<Simulator>();

            // 3. Verify your configuration
            container.Verify();
        }
    }
}
