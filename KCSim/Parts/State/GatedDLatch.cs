using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class GatedDLatch : StatefulGate
    {
        public readonly Axle Enable;
        public readonly Axle Data;

        // Hold onto references for the subcomponents for debugging.
        private readonly SRLatch srLatch;
        private readonly NandGate[] nandGates;

        public GatedDLatch(
            ICouplingService couplingService,
            IStateFactory stateFactory,
            IGateFactory gateFactory,
            string name = "gated D latch") : base(name)
        {
            // Create inputs unique to this stateful gate.
            Data = new Axle(name + "; data");
            Enable = new Axle(name + "; enable");

            // Create the SR latch.
            srLatch = stateFactory.CreateNewSRLatch(name + "; SR latch");
            couplingService.CreateNewLockedCoupling(Power, srLatch.Power);

            // Create 2 NAND gates.
            nandGates = new NandGate[4];
            for (int i = 0; i < 2; i++)
            {
                var nandGate = gateFactory.CreateNewNandGate(name: "NAND gate " + i);
                couplingService.CreateNewLockedCoupling(Power, nandGate.Power);
                nandGates[i] = nandGate;
            }

            // Wire up the inputs.
            couplingService.CreateNewLockedCoupling(Data, nandGates[0].InputA);
            couplingService.CreateNewLockedCoupling(Enable, nandGates[0].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, nandGates[1].InputA);
            couplingService.CreateNewLockedCoupling(Enable, nandGates[1].InputB);

            // Wire up the inputs to the SR latch.
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, srLatch.Set);
            couplingService.CreateNewLockedCoupling(nandGates[1].Output, srLatch.Reset);

            // Wire up the output.
            couplingService.CreateNewLockedCoupling(srLatch.Q, Q);
        }
    }
}
