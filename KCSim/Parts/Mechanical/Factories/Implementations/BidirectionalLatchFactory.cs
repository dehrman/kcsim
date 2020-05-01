namespace KCSim.Parts.Mechanical
{
    public class BidirectionalLatchFactory : IBidirectionalLatchFactory
    {
        private readonly ICouplingService couplingService;
        private readonly IRelayFactory relayFactory;

        public BidirectionalLatchFactory(
            ICouplingService couplingService,
            IRelayFactory relayFactory)
        {
            this.couplingService = couplingService;
            this.relayFactory = relayFactory;
        }

        public BidirectionalLatch CreateNew(string name = "")
        {
            return new BidirectionalLatch(couplingService, relayFactory, name: name);
        }
    }
}
