using System;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;

namespace KCSim.Physics.Couplings
{
    /**
     * This is a special kind of coupling used in situations when we need to bring a circuit into a known
     * initial state (e.g. in a ring oscillator).
     */
    public class InitialStateCoupling : Coupling
    {
        private readonly InitialState initialForce;
        private bool shouldApplyInitialForce;

        public InitialStateCoupling(
            InitialState initialForce,
            Torqueable output,
            string name = "initial state coupling"
            ) : base(new ExternalSwitch(initialForce), output, name)
        {
            this.initialForce = initialForce;
            shouldApplyInitialForce = true;
        }

        protected override Force GetInputForceOut()
        {
            return Force.ZeroForce;
        }

        protected override Force GetOutputForceOut()
        {
            if (shouldApplyInitialForce)
            {
                return initialForce;
            }
            else
            {
                return Force.ZeroForce;
            }
        }

        protected override void ReceiveForceOnInput(Force force)
        {
            if (!initialForce.Equals(force))
            {
                throw new InvalidOperationException("ReceiveForceOnInput invoked on an InitialStateCoupling"
                    + " with a force that does not match the initial force"
                    + "; initial force = " + initialForce + "; incoming force = " + force);
            }
        }

        protected override void ReceiveForceOnOutput(Force force)
        {
            if (!force.Equals(Force.ZeroForce) && !force.Equals(initialForce))
            {
                shouldApplyInitialForce = false;
            }
        }
    }
}
