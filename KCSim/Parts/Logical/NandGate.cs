using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class NandGate : Gate
    {
        public readonly Axle Power = new Axle("NAND gate power");
        public readonly Axle InputA = new Axle("NAND gate inputA");
        public readonly Axle InputB = new Axle("NAND gate inputB");
        public readonly Axle Output = new Axle("NAND gate output");

        public NandGate(
            ICouplingService couplingService,
            AndGate andGate,
            NotGate notGate)
            : base("NAND gate")
        {
            // An NAND gate can be constructed by simply inverting the output of an AND gate.
            couplingService.CreateNewLockedCoupling(InputA, andGate.InputA);
            couplingService.CreateNewLockedCoupling(InputB, andGate.InputB);
            couplingService.CreateNewLockedCoupling(andGate.Output, notGate.Input);
            couplingService.CreateNewLockedCoupling(notGate.Output, Output);

            // And don't forget to connect the power to the NAND gate. (The NOT gates don't need it.)
            couplingService.CreateNewLockedCoupling(Power, andGate.Power);
        }
    }
}
