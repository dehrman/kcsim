using System.Linq;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.LogicalBlocks
{
    public class Mux : LogicalBlock
    {
        public readonly Axle[] Select;

        private readonly ICouplingService couplingService;
        private readonly IGateFactory gateFactory;

        public Mux(
            ICouplingService couplingService,
            IGateFactory gateFactory,
            int numInputs,
            string name = "mux") : base(
                  numInputs: numInputs,
                  numOutputs: 1,
                  name: name)
        {
            this.couplingService = couplingService;
            this.gateFactory = gateFactory;

            // Create the select bit lines.
            int numSelectBits = BitMath.GetNumSelectBitsRequired(numInputs);
            Select = Enumerable.Range(0, numSelectBits)
                .Select(i => new Axle(name + "; select bit " + i))
                .ToArray();

            // Create and wire up the multi-inpu OR gate.
            MultiInputGate<OrGate> orGate = gateFactory.CreateNewMultiInputOrGate(numInputs);
            couplingService.CreateNewLockedCoupling(Power, orGate.Power);

            // Create and wire up the multi-input AND gates.
            int numInputsPerAndGate = numSelectBits + 1;
            for (int i = 0; i < numInputs; i++)
            {
                var andGate = gateFactory.CreateNewMultiInputAndGate(numInputsPerAndGate);
                couplingService.CreateNewLockedCoupling(Power, andGate.Power);
                couplingService.CreateNewLockedCoupling(Inputs[i], andGate.Inputs[numSelectBits]);
                ConnectSelectLines(i, andGate);
                couplingService.CreateNewLockedCoupling(andGate.Output, orGate.Inputs[i]);
            }

            couplingService.CreateNewLockedCoupling(orGate.Output, Output);
        }

        private void ConnectSelectLines(int selectIndex, MultiInputGate<AndGate> andGate)
        {
            int numSelectBits = Select.Length;

            // Here, we're inverting the select index to get a set of binary value that tell us
            // whether or not a given select wire requires a not gate before connecting it to its
            // corresponding input on the provided AND gate.
            //
            // As an example, if the select index is 6, that's binary 110. The 6th AND gate should
            // be enabled iff the select lines are {1, 1, 0}. Therefore, we need a NOT gate on the
            // lower bit of these select lines.
            //
            // As another example, if the select index were 9, that's binary 1001. So we'd need
            // NOT gates on the first and last select lines but not the middle two.
            bool[] bitRequiresNotGate = BitMath.GetBitVector(numSelectBits, ~selectIndex);
            
            for (int i = 0; i < numSelectBits; i++)
            {
                if (bitRequiresNotGate[i])
                {
                    var notGate = gateFactory.CreateNewNotGate();
                    couplingService.CreateNewLockedCoupling(Select[i], notGate.Input);
                    couplingService.CreateNewLockedCoupling(notGate.Output, andGate.Inputs[i]);
                }
                else
                {
                    couplingService.CreateNewLockedCoupling(Select[i], andGate.Inputs[i]);
                }

                
            }
        }
    }
}
