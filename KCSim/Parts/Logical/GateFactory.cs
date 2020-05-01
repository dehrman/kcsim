using System.Linq;
using KCSim.Parts.Mechanical;

namespace KCSim.Parts.Logical
{
    /**
    * The AND gate is the funadmental logic unit of all gates. Given that a NOT gate is trivial
    * (two coupled gears), we construct all other gates from AND and NOT gates.
    * 
    * Note that in the physical machine, an AND gate is also the standard module size. Therefore,
    * if a gate requires, say, three AND gates for its construction, it can be assumed that that
    * gate requires three standard modules of parts, space, and power.
    */
    public class GateFactory : IGateFactory
    {
        private readonly ICouplingService couplingService;
        private readonly IBidirectionalLatchFactory bidirectionalLatchFactory;
        private readonly IGateMonitor gateMonitor;

        public GateFactory(
            ICouplingService couplingService,
            IBidirectionalLatchFactory bidirectionalLatchFactory,
            IGateMonitor gateMonitor)
        {
            this.couplingService = couplingService;
            this.bidirectionalLatchFactory = bidirectionalLatchFactory;
            this.gateMonitor = gateMonitor;
        }

        /**
         * Trivial construction—no standard module required
         */
        public NotGate CreateNewNotGate(bool doMonitor = true, string name = "NOT gate")
        {
            var gate = new NotGate(couplingService, name);
            if (doMonitor)
            {
                gateMonitor.RegisterGate(gate);
            }
            return gate;
        }

        /**
         * Requires one standard module
         */
        public AndGate CreateNewAndGate(bool doMonitor = true, string name = "AND gate")
        {
            var gate = new AndGate(couplingService, bidirectionalLatchFactory, name);
            if (doMonitor)
            {
                gateMonitor.RegisterGate(gate);
            }
            return gate;
        }

        /**
         * Requires one standard module
         */
        public OrGate CreateNewOrGate(bool doMonitor = true, string name = "OR gate")
        {
            var andGate = CreateNewAndGate(doMonitor: false, name: name + "; AND gate");
            var notGateInputA = CreateNewNotGate(doMonitor: false, name: name + "; NOT gate input A");
            var notGateInputB = CreateNewNotGate(doMonitor: false, name: name + "; NOT gate input B");
            var notGateOutput = CreateNewNotGate(doMonitor: false, name: name + "; NOT gate output");
            var gate = new OrGate(couplingService, andGate, notGateInputA, notGateInputB, notGateOutput, name);
            if (doMonitor)
            {
                gateMonitor.RegisterGate(gate);
            }
            return gate;
        }

        /**
         * Requires one standard module
         */
        public NandGate CreateNewNandGate(bool doMonitor = true, string name = "NAND gate")
        {
            var andGate = CreateNewAndGate(doMonitor: false, name: name + "; AND gate");
            var notGate = CreateNewNotGate(doMonitor: false, name: name + "; AND gate");
            var gate = new NandGate(couplingService, andGate, notGate, name);
            if (doMonitor)
            {
                gateMonitor.RegisterGate(gate);
            }
            return gate;
        }

        /**
         * Requires three standard modules
         */
        public XorGate CreateNewXorGate(bool doMonitor = true, string name = "XOR gate")
        {
            var nandGate = CreateNewNandGate(doMonitor: false);
            var andGate = CreateNewAndGate(doMonitor: false);
            var orGate = CreateNewOrGate(doMonitor: false);
            var gate = new XorGate(couplingService, nandGate, andGate, orGate, name);
            if (doMonitor)
            {
                gateMonitor.RegisterGate(gate);
            }
            return gate;
        }

        public NorGate CreateNewNorGate(bool doMonitor = true, string name = "NOR gate")
        {
            var orGate = CreateNewOrGate(doMonitor: false);
            var notGate = CreateNewNotGate(doMonitor: false);
            var gate = new NorGate(couplingService, orGate, notGate, name);
            if (doMonitor)
            {
                gateMonitor.RegisterGate(gate);
            }
            return gate;
        }

        public Buffer CreateNewBuffer(bool doMonitor = true, string name = "buffer")
        {
            var gate = new Buffer(couplingService, bidirectionalLatchFactory, name);
            if (doMonitor)
            {
                gateMonitor.RegisterGate(gate);
            }
            return gate;
        }

        public MultiInputGate<AndGate> CreateNewMultiInputAndGate(int numInputs, bool doMonitor = true)
        {
            int numGates = numInputs - 1;
            var gates = Enumerable.Range(0, numInputs - 1)
                .Select(x => CreateNewAndGate(doMonitor: true))
                .ToArray();
            var gate = new MultiInputGate<AndGate>(numInputs, gates, couplingService, "multi-input AND gate");
            if (doMonitor)
            {
                gateMonitor.RegisterGate(gate);
            }
            return gate;
        }

        public MultiInputGate<OrGate> CreateNewMultiInputOrGate(int numInputs, bool doMonitor = true)
        {
            int numGates = numInputs - 1;
            var gates = Enumerable.Range(0, numInputs - 1)
                .Select(x => CreateNewOrGate(doMonitor: true))
                .ToArray();
            var gate = new MultiInputGate<OrGate>(numInputs, gates, couplingService, "multi-input OR gate");
            if (doMonitor)
            {
                gateMonitor.RegisterGate(gate);
            }
            return gate;
        }
    }
}
