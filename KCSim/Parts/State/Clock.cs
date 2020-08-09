using System.Collections.Generic;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public class Clock
    {
        public delegate void OnClockEdgeDelegate(ClockEdge clockEdge);
        public readonly ISet<OnClockEdgeDelegate> OnClockEdgeDelegateSet = new HashSet<OnClockEdgeDelegate>();

        public readonly ClockLevel Level;

        public ICouplingService couplingService;

        public Clock(
            Power power,
            IOscillator oscillator)
        {
            oscillator.CouplePowerInput(power);
            oscillator.GetOnQChangedDelegateSet().Add(OnQChanged);
            Level = new ClockLevel();
        }

        private void OnQChanged(Force oldQ, Force newQ)
        {
            Level.Update(newQ);

            ClockEdge clockEdge;
            if (oldQ.Velocity < 0 && newQ.Velocity > 0)
            {
                clockEdge = ClockEdge.Rising;
            }
            else if (oldQ.Velocity > 0 && newQ.Velocity < 0)
            {
                clockEdge = ClockEdge.Falling;
            }
            else
            {
                return;
            }

            // Invoke listeners for clock edges. Note that there is not physical equivalent for this logic;
            // this is used only for simulation purposes (e.g. to pause on rising clock edges).
            ISet<OnClockEdgeDelegate> copyOfDelegates = new HashSet<OnClockEdgeDelegate>(OnClockEdgeDelegateSet);
            foreach (OnClockEdgeDelegate handler in copyOfDelegates)
            {
                handler(clockEdge);
            }
        }
    }
}
