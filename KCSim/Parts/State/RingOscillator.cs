using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;
using static KCSim.Parts.State.IOscillator;

namespace KCSim.Parts.State
{
    public class RingOscillator : IOscillator
    {
        public readonly Axle Power = new Axle("ring oscillator power");
        public readonly Axle Q = new Axle("ring oscillator Q");

        private readonly ISet<OnQChangedDelegate> onQChangedDelegateSet = new HashSet<OnQChangedDelegate>();

        public RingOscillator(
            ICouplingService couplingService,
            IGateFactory gateFactory,
            int numGates)
        {
            Contract.Requires(numGates > 0, "A ring oscillator requires at least one gate.");

            // Create the gates.
            var buffers = Enumerable.Range(0, numGates)
                .Select(x => gateFactory.CreateNewBuffer()).ToArray();
            var notGates = Enumerable.Range(0, numGates)
                .Select(x => gateFactory.CreateNewNotGate()).ToArray();

            // Connect the first gate to the output.
            couplingService.CreateNewLockedCoupling(Q, buffers[0].Input);

            // And now invert the output of that first gate.
            couplingService.CreateNewLockedCoupling(buffers[0].Output, notGates[0].Input);
            
            // Connect the gates to each other.
            for (int i = 1; i < numGates; i++)
            {
                couplingService.CreateNewLockedCoupling(notGates[i - 1].Output, buffers[i].Input);
                couplingService.CreateNewLockedCoupling(buffers[i].Output, notGates[i].Input);
            }

            // Hook up the last gate to the output.
            couplingService.CreateNewLockedCoupling(notGates.Last().Output, Q);

            // Connect power to the buffers.
            System.Array.ForEach(buffers, buffer => couplingService.CreateNewLockedCoupling(Power, buffer.Power));

            // Attach a listener to the output. This has no physical-world equivalent but rather is just used
            // for the simulator.
            var qListener = new QListener(onQChangedDelegateSet);
            couplingService.CreateNewLockedCoupling(Q, qListener);

            // Finally, set the oscillator's first gate to a known initial state.
            couplingService.CreateNewInitialStateCoupling(new InitialState(), buffers[0].Input);
        }

        public ISet<OnQChangedDelegate> GetOnQChangedDelegateSet()
        {
            return onQChangedDelegateSet;
        }

        private class QListener : Axle
        {
            private readonly ISet<OnQChangedDelegate> onQChangedDelegateSet;

            public QListener(ISet<OnQChangedDelegate> onQChangedDelegateSet) : base()
            {
                this.onQChangedDelegateSet = onQChangedDelegateSet;
            }

            public override bool UpdateForce(Torqueable source, Force force)
            {
                Force oldQ = GetNetForce();
                if (!base.UpdateForce(source, force))
                {
                    return false;
                }
                Force newQ = GetNetForce();
                
                // We must create a copy of the delegates before iterating through them in the event that the set is
                // modified from outside this class during the iteration.
                ISet<OnQChangedDelegate> copyOfDelegates = new HashSet<OnQChangedDelegate>(onQChangedDelegateSet);
                foreach (OnQChangedDelegate handler in copyOfDelegates)
                {
                    handler(oldQ, newQ);
                }
                return true;
            }
        }
    }
}
