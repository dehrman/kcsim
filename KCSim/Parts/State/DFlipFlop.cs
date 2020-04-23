using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.State
{
    public class DFlipFlop
    {
        public readonly Axle Power = new Axle("gated D latch power");
        public readonly Axle DataIn = new Axle("gated D latch data in");
        public readonly Axle WriteEnable = new Axle("gated D latch write enable");
        public readonly Axle DataOut = new Axle("gated D latch data out");
        public readonly Axle DataOutInverse = new Axle("gated D latch data out inverse");

        public DFlipFlop(
            ICouplingService couplingService,
            IGateFactory gateFactory)
        {
            // Create 4 NAND gates.
            NandGate[] nandGates = Enumerable.Range(0, 4)
                .Select(x => gateFactory.CreateNewNandGate()).ToArray();

            // Connect the power to the gates.
            System.Array.ForEach(nandGates, gate => couplingService.CreateNewLockedCoupling(Power, gate.Power));

            couplingService.CreateNewLockedCoupling(DataIn, nandGates[0].InputA);
            couplingService.CreateNewLockedCoupling(WriteEnable, nandGates[0].InputB);
            couplingService.CreateNewLockedCoupling(WriteEnable, nandGates[1].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, nandGates[1].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, nandGates[2].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[2].Output, nandGates[3].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[2].Output, DataOut);
            couplingService.CreateNewLockedCoupling(nandGates[3].Output, DataOutInverse);
            couplingService.CreateNewLockedCoupling(nandGates[3].Output, nandGates[2].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[1].Output, nandGates[3].InputB);
        }
    }
}
