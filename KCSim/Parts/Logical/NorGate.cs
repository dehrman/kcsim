using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class NorGate : BinaryInputGate
    {
        public NorGate(
            ICouplingService couplingService,
            OrGate orGate,
            NotGate notGate,
            string name = "NOR gate")
            : base(name)
        {
            // An NOR gate can be constructed from an OR gate and a NOT gate.
            couplingService.CreateNewLockedCoupling(InputA, orGate.InputA);
            couplingService.CreateNewLockedCoupling(InputB, orGate.InputB);
            couplingService.CreateNewLockedCoupling(orGate.Output, notGate.Input);
            couplingService.CreateNewLockedCoupling(notGate.Output, Output);

            // And don't forget to connect the power to the OR gate. (The NOT gate doesn't need power.)
            couplingService.CreateNewLockedCoupling(Power, orGate.Power);
        }

        public override bool RequiresPower()
        {
            return true;
        }
    }
}
