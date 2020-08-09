using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    /**
     * A gated D latch based on an SR NOR latch
     */
    public class GatedDLatch : StatefulGate
    {
        public readonly Axle Enable;
        public readonly Axle Data;

        // Hold onto references for the subcomponents for debugging.
        private readonly AndGate[] andGates;
        private readonly NotGate notGate;
        private readonly SRLatch srLatch;

        public GatedDLatch(
            ICouplingService couplingService,
            IStateFactory stateFactory,
            IGateFactory gateFactory,
            string name = "gated D latch") : base(name)
        {
            // Create inputs unique to this stateful gate.
            Data = new Axle(name + "; data");
            Enable = new Axle(name + "; enable");

            // Create 2 AND gates.
            andGates = new AndGate[2];
            for (int i = 0; i < 2; i++)
            {
                var andGate = gateFactory.CreateNewAndGate(name: name + "; AND gate " + i);
                couplingService.CreateNewLockedCoupling(Power, andGate.Power);
                andGates[i] = andGate;
            }

            // Create the NOT gate that will invert the data input for the first AND gate.
            notGate = gateFactory.CreateNewNotGate(name: name + "; NOT gate");

            // Create the SR latch.
            srLatch = stateFactory.CreateNewSRLatch(name + "; SR latch");
            couplingService.CreateNewLockedCoupling(Power, srLatch.Power);

            // Wire up the inputs.
            couplingService.CreateNewLockedCoupling(Data, notGate.Input);
            couplingService.CreateNewLockedCoupling(notGate.Output, andGates[0].InputA);
            couplingService.CreateNewLockedCoupling(Enable, andGates[0].InputB);
            couplingService.CreateNewLockedCoupling(Enable, andGates[1].InputA);
            couplingService.CreateNewLockedCoupling(Data, andGates[1].InputB);

            // Wire up the inputs to the SR latch.
            couplingService.CreateNewLockedCoupling(andGates[0].Output, srLatch.Reset);
            couplingService.CreateNewLockedCoupling(andGates[1].Output, srLatch.Set);

            // Wire up the output.
            couplingService.CreateNewLockedCoupling(srLatch.Q, Q);
        }
    }
}
