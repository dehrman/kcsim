using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class GatedDLatch : StatefulGate
    {
        public readonly Axle Enable;

        public GatedDLatch(
            ICouplingService couplingService,
            IGateFactory gateFactory,
            string name = "gated D latch") : base(name)
        {
            // Create inputs unique to this stateful gate.
            Enable = new Axle(name + "; enable");

            // Create 4 NAND gates.
            NandGate[] nandGates = new NandGate[4];
            for (int i = 0; i < 4; i++)
            {
                var nandGate = gateFactory.CreateNewNandGate(name: "NAND gate " + i);
                couplingService.CreateNewLockedCoupling(Power, nandGate.Power);
                nandGates[i] = nandGate;
            }

            couplingService.CreateNewLockedCoupling(Data, nandGates[0].InputA);
            couplingService.CreateNewLockedCoupling(Enable, nandGates[0].InputB);
            couplingService.CreateNewLockedCoupling(Enable, nandGates[1].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, nandGates[1].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, nandGates[2].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[2].Output, nandGates[3].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[2].Output, Output);
            couplingService.CreateNewLockedCoupling(nandGates[3].Output, OutputInverse);
            couplingService.CreateNewLockedCoupling(nandGates[3].Output, nandGates[2].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[1].Output, nandGates[3].InputB);
        }
    }
}
