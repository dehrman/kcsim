namespace KCSim.Parts.Mechanical
{
    class BidirectionalLatchFactory : IBidirectionalLatchFactory
    {
        private readonly IRelayFactory relayFactory;

        BidirectionalLatchFactory(IRelayFactory relayFactory)
        {
            this.relayFactory = relayFactory;
        }

        public BidirectionalLatch CreateNew()
        {
            return new BidirectionalLatch(relayFactory);
        }
    }
}
