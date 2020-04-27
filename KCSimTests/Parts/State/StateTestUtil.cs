using System;
using KCSim;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;

namespace KCSimTests.Parts.State
{
    public class StateTestUtil
    {
        public StateTestUtil()
        {

        }

        public static void ToggleClockFrom_NegativeToPositive(
            ExternalSwitch clock,
            ForceEvaluator forceEvaluator)
        {
            clock.Force = new Force(-1);
            forceEvaluator.EvaluateForces();
            clock.Force = new Force(1);
            forceEvaluator.EvaluateForces();
        }
    }
}
