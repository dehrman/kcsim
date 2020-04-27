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
        public readonly Axle SetInverse;
        public readonly Axle ResetInverse;

        public SRLatch(
            ICouplingService couplingService,
            IGateFactory gateFactory,
            string name = "SR latch") : base(name)
        {
            // Create inputs unique to this stateful gate.
            SetInverse = new Axle(name + "; set inverse");
            ResetInverse = new Axle(name + "; reset inverse");

            // Create 2 NAND gates.
            NandGate[] nandGates = new NandGate[2];
            for (int i = 0; i < 2; i++)
            {
                var nandGate = gateFactory.CreateNewNandGate(name: "NAND gate " + i);
                couplingService.CreateNewLockedCoupling(Power, nandGate.Power);
                nandGates[i] = nandGate;
            }

            // Wire up the NAND gates.
            couplingService.CreateNewLockedCoupling(SetInverse, nandGates[0].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[1].Output, nandGates[0].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, Q);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, nandGates[1].InputA);
            couplingService.CreateNewLockedCoupling(ResetInverse, nandGates[1].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[1].Output, QInverse);
        }
    }
}
