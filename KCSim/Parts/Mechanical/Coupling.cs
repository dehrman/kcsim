using System;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical
{
    /**
     * The purpose of this class is to define and manage how forces are transferred between two Torqueables
     */
    public class Coupling<Input, Output>
        where Input : Torqueable
        where Output : Torqueable
    {
        private readonly Input input;
        private readonly Output output;
        private readonly double inputToOutputRatio;
        private readonly CouplingType couplingType;
        private readonly string name;

        private readonly CouplingForceEvaluator<Input, Output> forceEvaluator;

        private Force mostRecentInputToOutputForce = Force.ZeroForce;
        private Force mostRecentOutputToInputForce = Force.ZeroForce;

        public Coupling(Input input, Output output, double inputToOutputRatio, CouplingType couplingType, String name = "")
        {
            this.input = input;
            this.output = output;
            this.inputToOutputRatio = inputToOutputRatio;
            this.couplingType = couplingType;
            this.name = name;

            this.forceEvaluator = new CouplingForceEvaluator<Input, Output>(couplingType, inputToOutputRatio);

            EvaluateForces();
            AddListeners();
        }

        /**
         * Remove any coupled forces, to be called just before this object is destroyed.
         */
        public void Remove()
        {
            RemoveListeners();
            output.RemoveForce(mostRecentInputToOutputForce);
            input.RemoveForce(mostRecentOutputToInputForce);
        }

        private void EvaluateForces()
        {
            RemoveListeners();

            Tuple<Force, Force> newForces;
            try
            {
                newForces = forceEvaluator.EvaluateForces(input, output);
            }
            catch (DestructiveForceException e)
            {
                throw new DestructiveForceException("Exception in " + ToString(), e);
            }

            if (newForces.Item1 != mostRecentInputToOutputForce)
            {
                output.RemoveForce(mostRecentInputToOutputForce);
                mostRecentInputToOutputForce = newForces.Item1;
                output.AddForce(mostRecentInputToOutputForce);
            }
            if (newForces.Item2 != mostRecentOutputToInputForce)
            {
                input.RemoveForce(mostRecentOutputToInputForce);
                mostRecentOutputToInputForce = newForces.Item2;
                input.AddForce(mostRecentOutputToInputForce);
            }
            AddListeners();
        }

        private void OnInputNetForceChanged(Torqueable t)
        {
            // Before evaluating net forces, we should first remove the previous net force that the input was applying
            // to the output.
            if (mostRecentInputToOutputForce != Force.ZeroForce)
            {
                output.OnNetForceChangedDelegateSet.Remove(OnOutputNetForceChanged);
                output.RemoveForce(mostRecentInputToOutputForce);
                output.OnNetForceChangedDelegateSet.Add(OnOutputNetForceChanged);
            }

            EvaluateForces();
        }

        private void OnOutputNetForceChanged(Torqueable t)
        {
            // Before evaluating net forces, we should first remove the previous net force that the output was applying
            // to the input.
            if (mostRecentOutputToInputForce != Force.ZeroForce)
            {
                input.OnNetForceChangedDelegateSet.Remove(OnInputNetForceChanged);
                input.RemoveForce(mostRecentOutputToInputForce);
                input.OnNetForceChangedDelegateSet.Add(OnInputNetForceChanged);
            }

            EvaluateForces();
        }

        private void AddListeners()
        {
            input.OnNetForceChangedDelegateSet.Add(OnInputNetForceChanged);
            output.OnNetForceChangedDelegateSet.Add(OnOutputNetForceChanged);
        }

        private void RemoveListeners()
        {
            input.OnNetForceChangedDelegateSet.Remove(OnInputNetForceChanged);
            output.OnNetForceChangedDelegateSet.Remove(OnOutputNetForceChanged);
        }

        public static Coupling<I, O> NewGearCoupling<I, O>(I input, O output, CouplingType couplingType, String name = "")
            where I : Gear
            where O : Gear
        {
            return new Coupling<I, O>(input, output, (double)input.GetNumTeeth() / (double)output.GetNumTeeth(), couplingType, name);
        }

        public static Coupling<I, O> NewLockedAxleCoupling<I, O>(I input, O output, String name = "")
            where I : Axle
            where O : Axle
        {
            return new Coupling<I, O>(input, output, 1, CouplingType.BidirectionalSymmetrical, name);
        }

        public static Coupling<I, O> NewLockedGearToAxleCoupling<I, O>(I input, O output, String name = "")
            where I : Gear
            where O : Axle
        {
            return new Coupling<I, O>(input, output, 1, CouplingType.BidirectionalSymmetrical, name);
        }

        public static Coupling<I, O> NewLockedAxleToGearCoupling<I, O>(I input, O output, String name = "")
            where I : Axle
            where O : Gear
        {
            return new Coupling<I, O>(input, output, 1, CouplingType.BidirectionalSymmetrical, name);
        }

        public override string ToString()
        {
            return "Coupling \"" + name + "\": {" + input + "; " + output + "; inputToOutputRatio=" + inputToOutputRatio
                + "; couplingType=" + couplingType;
        }
    }
}
