using System;
using System.Threading;
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
        private readonly IPaddleFactory paddleFactory;

        public Simulator(
            IStateFactory stateFactory,
            IGateFactory gateFactory,
            ICouplingService couplingService,
            ForceEvaluator forceEvaluator,
            IPaddleFactory paddleFactory
            )
        {
            this.stateFactory = stateFactory;
            this.gateFactory = gateFactory;
            this.couplingService = couplingService;
            this.forceEvaluator = forceEvaluator;
            this.paddleFactory = paddleFactory;
        }

        public void RunSimulation()
        {
            ExternalSwitch Power = new ExternalSwitch(new Force(1), "power");
            ExternalSwitch Set = new ExternalSwitch(new Force(1), "set");
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
            Thread.Sleep(1);
            forceEvaluator.EvaluateForces();
            // Thread.Sleep(2000);
            // forceEvaluator.EvaluateForces();

            Console.WriteLine("AND output: " + andGate.Output);
            Console.WriteLine("SR latch output: " + srLatch.Q);

            int enableForce = 1;
            int latchedInputForce = 1;

            ExternalSwitch enable = new ExternalSwitch();
            ExternalSwitch input = new ExternalSwitch();
            enable.Force = GetEnableForce(enableForce);
            input.Force = GetLatchedInputForce(latchedInputForce);
            var relay = GetRelay(enableForce, latchedInputForce, enable, input);
            // couplingMonitor.OnCoupledToInput(relay.OutputGear, (coupling) => Console.WriteLine("output gear has been connected to input via new coupling: " + coupling));

            forceEvaluator.EvaluateForces();
            Thread.Sleep(100);
            forceEvaluator.EvaluateForces();

            Console.WriteLine("Expecting " + GetLatchedInputForce(latchedInputForce) + "; received " + relay.OutputAxle.GetNetForce());
        }


        private Force GetEnableForce(int enableForce)
        {
            return new Force(enableForce);
        }

        private Force GetDisableForce(int enableForce)
        {
            return new Force(enableForce * -1);
        }

        private Force GetLatchedInputForce(int latchedInputForce)
        {
            return new Force(latchedInputForce);
        }

        private Force GetNonLatchedInputForce(int latchedInputForce)
        {
            return new Force(latchedInputForce * -1);
        }

        private Relay GetRelay(int enableForce, int latchedInputForce, Torqueable enable, Torqueable input)
        {
            var relay = new Relay(
                couplingService: couplingService,
                paddleFactory: paddleFactory,
                enableDirection: enableForce < 0 ? Direction.Negative : Direction.Positive,
                inputDirection: latchedInputForce < 0 ? Direction.Negative : Direction.Positive,
                name: (enableForce < 0 ? "negative" : "positive") + " relay");

            couplingService.CreateNewLockedCoupling(enable, relay.Enable);
            couplingService.CreateNewLockedCoupling(input, relay.InputAxle);

            return relay;
        }
    }
}
