using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class XorGate : Gate
    {
        public XorGate(
            ICouplingService couplingService,
            NandGate nandGate,
            AndGate andGate,
            OrGate orGate)
            : base("XOR gate")
        {
            // An XOR gate can be constructed from a NAND gate, an AND gate, and an OR gate.
            couplingService.CreateNewLockedCoupling(InputA, nandGate.InputA);
            couplingService.CreateNewLockedCoupling(InputB, nandGate.InputB);
            couplingService.CreateNewLockedCoupling(InputA, orGate.InputA);
            couplingService.CreateNewLockedCoupling(InputB, orGate.InputB);
            couplingService.CreateNewLockedCoupling(nandGate.Output, andGate.InputA);
            couplingService.CreateNewLockedCoupling(orGate.Output, andGate.InputB);
            couplingService.CreateNewLockedCoupling(andGate.Output, Output);

            // And don't forget to connect the power to the NAND, AND, and OR gates.
            couplingService.CreateNewLockedCoupling(Power, nandGate.Power);
            couplingService.CreateNewLockedCoupling(Power, andGate.Power);
            couplingService.CreateNewLockedCoupling(Power, orGate.Power);
        }
    }
}
