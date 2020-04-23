using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class NandGate : Gate
    {
        public NandGate(
            ICouplingService couplingService,
            AndGate andGate,
            NotGate notGate,
            string name = "NAND gate")
            : base(name)
        {
            // An NAND gate can be constructed by simply inverting the output of an AND gate.
            couplingService.CreateNewLockedCoupling(InputA, andGate.InputA, name + "; input A to AND gate input A");
            couplingService.CreateNewLockedCoupling(InputB, andGate.InputB, name + "; input B to AND gate input B");
            couplingService.CreateNewLockedCoupling(andGate.Output, notGate.Input, name + "; AND gate output to NOT gate input");
            couplingService.CreateNewLockedCoupling(notGate.Output, Output, name + "; NOT gate output to output");

            // And don't forget to connect the power to the NAND gate. (The NOT gates don't need it.)
            couplingService.CreateNewLockedCoupling(Power, andGate.Power);
        }
    }
}
