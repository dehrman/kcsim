using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class SRLatch : StatefulGate
    {
        public readonly Axle Set;
        public readonly Axle Reset;

        // Hold onto references for the subcomponents for debugging.
        private readonly OrGate orGate;
        private readonly AndGate andGate;
        private readonly NotGate notGate;

        public SRLatch(
            ICouplingService couplingService,
            IGateFactory gateFactory,
            string name = "SR latch") : base(name)
        {
            // Create inputs unique to this stateful gate.
            Set = new Axle(name + "; set");
            Reset = new Axle(name + "; reset");

            // Create the gates.
            orGate = gateFactory.CreateNewOrGate(name: name + "; OR gate");
            couplingService.CreateNewLockedCoupling(Power, orGate.Power);
            andGate = gateFactory.CreateNewAndGate(name: name + "; AND gate");
            couplingService.CreateNewLockedCoupling(Power, andGate.Power);
            notGate = gateFactory.CreateNewNotGate(name: name + "; NOT gate");

            // Wire up the gates.
            couplingService.CreateNewLockedCoupling(Set, orGate.InputB);
            couplingService.CreateNewLockedCoupling(Reset, notGate.Input);
            couplingService.CreateNewLockedCoupling(orGate.Output, andGate.InputA);
            couplingService.CreateNewLockedCoupling(notGate.Output, andGate.InputB);
            couplingService.CreateNewLockedCoupling(andGate.Output, orGate.InputA);
            couplingService.CreateNewLockedCoupling(andGate.Output, Q);
        }
    }
}
