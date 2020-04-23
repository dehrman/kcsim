using KCSim.Parts.Logical;

namespace KCSim.Parts.State
{
    public class StateFactory : IStateFactory
    {
        private readonly ICouplingService couplingService;
        private readonly IGateFactory gateFactory;

        public StateFactory(ICouplingService couplingService, IGateFactory gateFactory)
        {
            this.couplingService = couplingService;
            this.gateFactory = gateFactory;
        }


        public SRLatch CreateNewSRLatch()
        {
            return new SRLatch(couplingService, gateFactory);
        }

        public GatedDLatch CreateNewGatedDLatch()
        {
            return new GatedDLatch(couplingService, gateFactory);
        }
    }
}
