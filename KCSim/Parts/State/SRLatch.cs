using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class SRLatch
    {
        public readonly Axle Power = new Axle(name: "SR latch power", (oldForce, newForce) =>
    System.Diagnostics.Debug.WriteLine("SR latch power changed from " + oldForce + " to " + newForce));
        public readonly Axle SetInverse = new Axle(name: "SR latch data in", (oldForce, newForce) =>
            System.Diagnostics.Debug.WriteLine("SR latch data in changed from " + oldForce + " to " + newForce));
        public readonly Axle ResetInverse = new Axle(name: "SR latch write enable", (oldForce, newForce) =>
            System.Diagnostics.Debug.WriteLine("SR latch write enable changed from " + oldForce + " to " + newForce));
        public readonly Axle DataOut = new Axle(name: "SR latch data out", (oldForce, newForce) =>
            System.Diagnostics.Debug.WriteLine("SR latch data out changed from " + oldForce + " to " + newForce));
        public readonly Axle DataOutInverse = new Axle(name: "SR latch data out inverse", (oldForce, newForce) =>
            System.Diagnostics.Debug.WriteLine("SR latch data out inverse changed from " + oldForce + " to " + newForce));

        public SRLatch(
            ICouplingService couplingService,
            IGateFactory gateFactory)
        {
            // Create 4 NAND gates.
            NandGate[] nandGates = new NandGate[2];
            for (int i = 0; i < 2; i++)
            {
                var nandGate = gateFactory.CreateNewNandGate(name: "NAND gate " + i);
                couplingService.CreateNewLockedCoupling(Power, nandGate.Power);
                nandGates[i] = nandGate;
            }

            couplingService.CreateNewLockedCoupling(SetInverse, nandGates[0].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[1].Output, nandGates[0].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, DataOut);
            
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, nandGates[1].InputA);
            couplingService.CreateNewLockedCoupling(ResetInverse, nandGates[1].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[1].Output, DataOutInverse);

            // We need to initialize the outputs into a known deterministic state.
            couplingService.CreateNewInitialStateCoupling(new Force(1), DataOut);
            couplingService.CreateNewInitialStateCoupling(new Force(-1), DataOutInverse);
        }
    }
}
