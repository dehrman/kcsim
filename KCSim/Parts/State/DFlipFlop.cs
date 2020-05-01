using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class DFlipFlop : StatefulGate
    {
        public readonly Axle Clock;
        public readonly Axle Data;

        // Hold onto references to the subcomponents for debugging.
        public readonly GatedDLatch[] Latches;
        public readonly NotGate[] NotGates;

        public DFlipFlop(
            ICouplingService couplingService,
            IGateFactory gateFactory,
            IStateFactory stateFactory,
            string name = "D flip-flop") : base(name)
        {
            // Create inputs unique to this stateful gate.
            Data = new Axle(name + "; data");
            Clock = new Axle(name + "; clock");

            // Create the gates.
            Latches = new GatedDLatch[2];
            NotGates = new NotGate[2];
            for (int i = 0; i < 2; i++)
            {
                Latches[i] = stateFactory.CreateNewGatedDLatch(name: name + "; gated D latch " + i);
                couplingService.CreateNewLockedCoupling(Power, Latches[i].Power);
                NotGates[i] = gateFactory.CreateNewNotGate(name: name + "; NOT gate " + i);
                couplingService.CreateNewLockedCoupling(NotGates[i].Output, Latches[i].Enable);
            }

            // Wire up the clock.
            couplingService.CreateNewLockedCoupling(Clock, NotGates[0].Input);
            couplingService.CreateNewLockedCoupling(NotGates[0].Output, NotGates[1].Input);

            // Wire up the data.
            couplingService.CreateNewLockedCoupling(Data, Latches[0].Data);
            couplingService.CreateNewLockedCoupling(Latches[0].Q, Latches[1].Data);

            // Wire up the output.
            couplingService.CreateNewLockedCoupling(Latches[1].Q, Q);
        }
    }
}
