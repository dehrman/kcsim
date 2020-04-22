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

        public GateFactory(
            ICouplingService couplingService,
            IBidirectionalLatchFactory bidirectionalLatchFactory)
        {
            this.couplingService = couplingService;
            this.bidirectionalLatchFactory = bidirectionalLatchFactory;
        }

        /**
         * Trivial construction—no standard module required
         */
        public NotGate CreateNewNotGate()
        {
            return new NotGate(couplingService);
        }

        /**
         * Requires one standard module
         */
        public AndGate CreateNewAndGate()
        {
            return new AndGate(couplingService, bidirectionalLatchFactory);
        }

        /**
         * Requires one standard module
         */
        public OrGate CreateNewOrGate()
        {
            var andGate = CreateNewAndGate();
            var notGateInputA = CreateNewNotGate();
            var notGateInputB = CreateNewNotGate();
            var notGateOutput = CreateNewNotGate();
            return new OrGate(couplingService, andGate, notGateInputA, notGateInputB, notGateOutput);
        }

        /**
         * Requires one standard module
         */
        public NandGate CreateNewNandGate()
        {
            var andGate = CreateNewAndGate();
            var notGate = CreateNewNotGate();
            return new NandGate(couplingService, andGate, notGate);
        }

        /**
         * Requires three standard modules
         */
        public XorGate CreateNewXorGate()
        {
            var nandGate = CreateNewNandGate();
            var andGate = CreateNewAndGate();
            var orGate = CreateNewOrGate();
            return new XorGate(couplingService, nandGate, andGate, orGate);
        }
    }
}
