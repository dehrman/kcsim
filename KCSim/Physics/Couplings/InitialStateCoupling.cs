using System;
using KCSim.Parts.Mechanical.Machines;

namespace KCSim.Physics.Couplings
{
    /**
     * This is a special kind of coupling used in situations when we need to bring a circuit into a known
     * initial state (e.g. in a ring oscillator).
     */
    public class InitialStateCoupling : Coupling
    {
        private readonly Force initialForce;
        private bool shouldApplyInitialForce;

        public InitialStateCoupling(
            Force initialForce,
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
            if (!Force.ZeroForce.Equals(initialForce))
            {
                shouldApplyInitialForce = true;
            }
        }
    }
}
