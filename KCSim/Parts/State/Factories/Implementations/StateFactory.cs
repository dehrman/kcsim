using KCSim.Parts.Logical;

namespace KCSim.Parts.State
{
    public class StateFactory : IStateFactory
    {
        private readonly ICouplingService couplingService;
        private readonly IGateFactory gateFactory;
        private readonly IStateMonitor stateMonitor;

        public StateFactory(
            ICouplingService couplingService,
            IGateFactory gateFactory,
            IStateMonitor stateMonitor)
        {
            this.couplingService = couplingService;
            this.gateFactory = gateFactory;
            this.stateMonitor = stateMonitor;
        }


        public SRLatch CreateNewSRLatch()
        {
            return stateMonitor.RegisterGate(new SRLatch(couplingService, gateFactory));
        }

        public GatedDLatch CreateNewGatedDLatch()
        {
            return stateMonitor.RegisterGate(new GatedDLatch(couplingService, gateFactory));
        }
    }
}
