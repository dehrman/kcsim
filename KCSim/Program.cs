using System;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical;
using KCSim.Parts.State;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using SimpleInjector;

namespace KCSim
{
    /// <summary>
    /// The entry point for the simulator.
    /// 
    /// All that this class does is set up the dependency injection framework, instantiate the simulator class, and
    /// defer to the simulator to start up.
    /// </summary>
    static class Program
    {
        // The stronger the power, the faster the computer will run. It sounds like a joke, but it's true. In practice
        // that means that the more torque we can get from the motors without reducing the angular velocity, the faster
        // the computer will be able to run.
        //
        // Note that here, "power" is still a rotational force, just like any other forces in the machine. The word
        // "power" is simply used to distinguish its purpose from other forces in the machine.
        private static readonly Force InitialPowerForce = new Force(1);

        // The single dependency injection container used by the entire simulator
        private static readonly Container container;

        /// <summary>
        /// The entry point (e.g. from the command line) for the whole simulator
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            Simulator simulator = container.GetInstance<Simulator>();
            simulator.Start();
        }

        static Program()
        {
            // 1. Create a new Simple Injector container
            container = new Container();

            // 2. Configure the container (register)
            container.Register<IMotionTimer, MotionTimer>();
            container.RegisterSingleton<MotionTimerFactory>();
            container.RegisterSingleton<IPaddleFactory, PaddleFactory>();
            container.RegisterSingleton<IPartsGraph, PartsGraph>();
            container.RegisterSingleton<IGateMonitor, GateMonitor>();
            container.RegisterSingleton<ICouplingMonitor, CouplingMonitor>();
            container.RegisterSingleton<IBidirectionalLatchFactory, BidirectionalLatchFactory>();
            container.RegisterSingleton<ICouplingService, CouplingService>();
            container.RegisterSingleton<ICouplingFactory, CouplingFactory>();
            container.RegisterSingleton<IRelayFactory, RelayFactory>();
            container.RegisterSingleton<IStateFactory, StateFactory>();
            container.RegisterSingleton<IGateFactory, GateFactory>();
            container.RegisterSingleton<IStateMonitor, StateMonitor>();
            container.RegisterSingleton<IOscillator, RingOscillator>();
            container.RegisterSingleton<Clock>();
            container.RegisterSingleton<ForceEvaluator>();
            container.RegisterSingleton<Power>(() => new Power(initialForce: InitialPowerForce));
            container.RegisterSingleton<Simulator>();

            // 3. Verify your configuration
            container.Verify();
        }
    }
}
