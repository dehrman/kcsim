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
        private readonly IPaddleFactory paddleFactory;
        private readonly IGateFactory gateFactory;
        private readonly IStateFactory stateFactory;
        private readonly ICouplingService couplingService;
        private readonly ForceEvaluator forceEvaluator;
        private readonly Clock clock;

        public Simulator(
            IPaddleFactory paddleFactory,
            IGateFactory gateFactory,
            IStateFactory stateFactory,
            ICouplingService couplingService,
            ForceEvaluator forceEvaluator,
            Clock clock
            )
        {
            this.stateFactory = stateFactory;
            this.gateFactory = gateFactory;
            this.couplingService = couplingService;
            this.forceEvaluator = forceEvaluator;
            this.paddleFactory = paddleFactory;
        }

        public void Start()
        {
            ExternalSwitch Power = new ExternalSwitch(new Force(1), "power");


            DFlipFlop flipFlop = stateFactory.CreateNewDFlipFlop();
            

            forceEvaluator.EvaluateForces();
            Thread.Sleep(1000);
            forceEvaluator.EvaluateForces();
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
