using System;
using System.Collections.Generic;

namespace KCSim.Physics.Couplings
{
    /**
     * The purpose of this class is to define and manage how forces are transferred between two Torqueables
     */
    public abstract class Coupling
    {
        public readonly Torqueable Input;
        public readonly Torqueable Output;
        private readonly string name;

        public Coupling(
            Torqueable input,
            Torqueable output,
            string name = "")
        {
            this.Input = input;
            this.Output = output;
            this.name = name;
        }

        public Torqueable GetOther(Torqueable torqueable)
        {
            if (torqueable == Input)
            {
                return Output;
            }
            else
            {
                return Input;
            }
        }

        /**
         * Receive a force on this coupling, for the given torqueable, and return a force to be applied
         * to the next node(s) in the structure.
         */
        public Force ReceiveForce(Torqueable torqueable, Force force)
        {
            if (torqueable == Input)
            {
                ReceiveForceOnInput(force);
                return GetOutputForceOut();
            }
            else if (torqueable == Output)
            {
                ReceiveForceOnOutput(force);
                return GetInputForceOut();
            }
            else
            {
                throw new InvalidOperationException("No equivalent Torqueable found in this coupling.");
            }
        }

        protected abstract void ReceiveForceOnInput(Force force);
        protected abstract void ReceiveForceOnOutput(Force force);
        protected abstract Force GetInputForceOut();
        protected abstract Force GetOutputForceOut();

        public override string ToString()
        {
            return "coupling from " + Input + " to " + Output;
        }

        public override bool Equals(object obj)
        {
            return obj is Coupling coupling &&
                   EqualityComparer<Torqueable>.Default.Equals(Input, coupling.Input) &&
                   EqualityComparer<Torqueable>.Default.Equals(Output, coupling.Output);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Input, Output);
        }
    }
}
