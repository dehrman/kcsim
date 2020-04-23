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
            return stateMonitor.RegisterStatefulGate(new SRLatch(couplingService, gateFactory));
        }

        public GatedDLatch CreateNewGatedDLatch()
        {
            return stateMonitor.RegisterStatefulGate(new GatedDLatch(couplingService, gateFactory));
        }
    }
}
