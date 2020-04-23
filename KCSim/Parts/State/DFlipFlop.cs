using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class DFlipFlop : StatefulGate
    {
        public readonly Axle Enable;

        public DFlipFlop(
            ICouplingService couplingService,
            IGateFactory gateFactory,
            string name = "D flip-flop") : base(name)
        {
            // Create inputs unique to this stateful gate.
            Enable = new Axle(name + "; enable");

            // Create 4 NAND gates.
            NandGate[] nandGates = Enumerable.Range(0, 4)
                .Select(x => gateFactory.CreateNewNandGate()).ToArray();

            // Connect the power to the gates.
            System.Array.ForEach(nandGates, gate => couplingService.CreateNewLockedCoupling(Power, gate.Power));

            couplingService.CreateNewLockedCoupling(Data, nandGates[0].InputA);
            couplingService.CreateNewLockedCoupling(Enable, nandGates[0].InputB);
            couplingService.CreateNewLockedCoupling(Enable, nandGates[1].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, nandGates[1].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[0].Output, nandGates[2].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[2].Output, nandGates[3].InputA);
            couplingService.CreateNewLockedCoupling(nandGates[2].Output, Output);
            couplingService.CreateNewLockedCoupling(nandGates[3].Output, OutputInverse);
            couplingService.CreateNewLockedCoupling(nandGates[3].Output, nandGates[2].InputB);
            couplingService.CreateNewLockedCoupling(nandGates[1].Output, nandGates[3].InputB);

            // Initialize the outputs into a known deterministic state.
            couplingService.CreateNewInitialStateCoupling(new InitialState(), Output);
            couplingService.CreateNewInitialStateCoupling(new Force(new InitialState().Velocity * -1), OutputInverse);
        }
    }
}
