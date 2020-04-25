using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public class NorGate : Gate
    {
        public readonly Axle Power = new Axle("NOR gate power");
        public readonly Axle InputA = new Axle("NOR gate inputA");
        public readonly Axle InputB = new Axle("NOR gate inputB");
        public readonly Axle Output = new Axle("NOR gate output");

        public NorGate(
            ICouplingService couplingService,
            OrGate orGate,
            NotGate notGate)
            : base("NOR gate")
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
