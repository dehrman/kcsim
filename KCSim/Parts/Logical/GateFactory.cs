using KCSim.Parts.Mechanical;

namespace KCSim.Parts.Logical
{
    public class GateFactory : IGateFactory
    {
        private readonly IBidirectionalLatchFactory bidirectionalLatchFactory;

        public GateFactory(IBidirectionalLatchFactory bidirectionalLatchFactory)
        {
            this.bidirectionalLatchFactory = bidirectionalLatchFactory;
        }

        public AndGate CreateNewAndGate()
        {
            return new AndGate(bidirectionalLatchFactory);
        }
    }
}
