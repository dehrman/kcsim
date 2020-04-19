using KCSim.Parts.Mechanical;

namespace KCSim.Parts.Logical
{
    public class GateFactory : IGateFactory
    {
        private readonly ICouplingService couplingService;
        private readonly IBidirectionalLatchFactory bidirectionalLatchFactory;

        public GateFactory(
            ICouplingService couplingService,
            IBidirectionalLatchFactory bidirectionalLatchFactory)
        {
            this.bidirectionalLatchFactory = bidirectionalLatchFactory;
        }

        public AndGate CreateNewAndGate()
        {
            return new AndGate(couplingService, bidirectionalLatchFactory);
        }
    }
}
