using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class GatedDLatch
    {
        public readonly Axle Power = new Axle(name: "gated D latch power", (oldForce, newForce) =>
            System.Diagnostics.Debug.WriteLine("gated D latch power changed from " + oldForce + " to " + newForce));
        public readonly Axle DataIn = new Axle(name: "gated D latch data in", (oldForce, newForce) =>
            System.Diagnostics.Debug.WriteLine("gated D latch data in changed from " + oldForce + " to " + newForce));
        public readonly Axle WriteEnable = new Axle(name: "gated D latch write enable", (oldForce, newForce) =>
            System.Diagnostics.Debug.WriteLine("gated D latch write enable changed from " + oldForce + " to " + newForce));
        public readonly Axle DataOut = new Axle(name: "gated D latch data out", (oldForce, newForce) =>
            System.Diagnostics.Debug.WriteLine("gated D latch data out changed from " + oldForce + " to " + newForce));
        public readonly Axle DataOutInverse = new Axle(name: "gated D latch data out inverse", (oldForce, newForce) =>
            System.Diagnostics.Debug.WriteLine("gated D latch data out inverse changed from " + oldForce + " to " + newForce));

        public GatedDLatch(
            ICouplingService couplingService,
            IGateFactory gateFactory)
        {
            // Create 4 NAND gates.
            NandGate[] nandGates = new NandGate[4];
            for (int i = 0; i < 4; i++)
            {
                var nandGate = gateFactory.CreateNewNandGate(name: "NAND gate " + i);
                couplingService.CreateNewLockedCoupling(Power, nandGate.Power);
                nandGates[i] = nandGate;
            }

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

            // We need to initialize the outputs into a known deterministic state.
            couplingService.CreateNewInitialStateCoupling(new Force(1), DataOut);
            couplingService.CreateNewInitialStateCoupling(new Force(-1), DataOutInverse);
        }
    }
}
