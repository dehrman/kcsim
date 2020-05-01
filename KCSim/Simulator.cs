using System;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;
using KCSim.Physics;

namespace KCSim
{
    public class Simulator
    {
        private readonly IStateFactory stateFactory;
        private readonly IGateFactory gateFactory;
        private readonly ICouplingService couplingService;
        private readonly ForceEvaluator forceEvaluator;

        public Simulator(
            IStateFactory stateFactory,
            IGateFactory gateFactory,
            ICouplingService couplingService,
            ForceEvaluator forceEvaluator
            )
        {
            this.stateFactory = stateFactory;
            this.gateFactory = gateFactory;
            this.couplingService = couplingService;
            this.forceEvaluator = forceEvaluator;
        }

        public void RunSimulation()
        {
            ExternalSwitch Power = new ExternalSwitch(new Force(1), "power");
            ExternalSwitch Set = new ExternalSwitch(new Force(-1), "set");
            ExternalSwitch Reset = new ExternalSwitch(new Force(1), "reset");

            SRLatch srLatch = stateFactory.CreateNewSRLatch();
            couplingService.CreateNewLockedCoupling(Power, srLatch.Power);
            couplingService.CreateNewLockedCoupling(Set, srLatch.Set);
            couplingService.CreateNewLockedCoupling(Reset, srLatch.Reset);

            AndGate andGate = gateFactory.CreateNewAndGate();
            couplingService.CreateNewLockedCoupling(Power, andGate.Power);
            couplingService.CreateNewLockedCoupling(Set, andGate.InputA);
            couplingService.CreateNewLockedCoupling(Reset, andGate.InputB);

            forceEvaluator.EvaluateForces();

            Console.WriteLine("AND output: " + andGate.Output);
            Console.WriteLine("SR latch output: " + srLatch.Q);
        }
    }
}
