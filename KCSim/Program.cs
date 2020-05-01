using System;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical;
using KCSim.Parts.State;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using SimpleInjector;

namespace KCSim
{
    static class Program
    {
        private static readonly Container container;

        static void Main(string[] args)
        {
            Simulator simulator = container.GetInstance<Simulator>();

            simulator.RunSimulation();
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
            container.RegisterSingleton<ForceEvaluator>();
            container.RegisterSingleton<Simulator>();

            // 3. Verify your configuration
            container.Verify();
        }
    }
}
