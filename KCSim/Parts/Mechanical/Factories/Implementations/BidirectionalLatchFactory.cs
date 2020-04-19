namespace KCSim.Parts.Mechanical
{
    class BidirectionalLatchFactory : IBidirectionalLatchFactory
    {
        private readonly ICouplingService couplingService;
        private readonly IRelayFactory relayFactory;

        BidirectionalLatchFactory(
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
