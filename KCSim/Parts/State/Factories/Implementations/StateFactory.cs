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


        public SRLatch CreateNewSRLatch(string name = "SR latch")
        {
            return stateMonitor.RegisterGate(new SRLatch(couplingService, gateFactory, name));
        }

        public GatedDLatch CreateNewGatedDLatch(string name = "gated D latch")
        {
            var gate = new GatedDLatch(
                couplingService: couplingService,
                stateFactory: this,
                gateFactory: gateFactory,
                name: name);
            return stateMonitor.RegisterGate(gate);
        }
    }
}
