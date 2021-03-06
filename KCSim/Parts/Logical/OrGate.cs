﻿using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class OrGate : BinaryInputGate
    {
        public OrGate(
            ICouplingService couplingService,
            AndGate andGate,
            NotGate notGateInputA,
            NotGate notGateInputB,
            NotGate notGateOutput,
            string name = "OR gate") : base (name)
        {
            // An OR gate can be constructed by simply inverting the inputs to an AND gate
            // and inverting its output.
            couplingService.CreateNewLockedCoupling(InputA, notGateInputA.Input);
            couplingService.CreateNewLockedCoupling(notGateInputA.Output, andGate.InputA);
            couplingService.CreateNewLockedCoupling(InputB, notGateInputB.Input);
            couplingService.CreateNewLockedCoupling(notGateInputB.Output, andGate.InputB);
            couplingService.CreateNewLockedCoupling(andGate.Output, notGateOutput.Input);
            couplingService.CreateNewLockedCoupling(notGateOutput.Output, Output);

            // And don't forget to connect the power to the AND gate. (The NOT gates don't need it.)
            couplingService.CreateNewLockedCoupling(Power, andGate.Power);
        }

        public override bool RequiresPower()
        {
            return true;
        }
    }
}
