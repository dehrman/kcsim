using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Parts.State;
using KCSim.Physics;
using KCSim.Physics.Couplings;

namespace KCSim
{
    /**
     * A singleton service that creates and tracks couplings between parts.
     */
    public class CouplingService : ICouplingService
    {
        private readonly ICouplingMonitor couplingMonitor;
        private readonly ICouplingFactory couplingFactory;

        public CouplingService(
            ICouplingMonitor couplingMonitor,
            ICouplingFactory couplingFactory)
        {
            this.couplingMonitor = couplingMonitor;
            this.couplingFactory = couplingFactory;
        }

        public void RemoveCoupling(Coupling coupling)
        {
            couplingMonitor.RemoveCoupling(coupling);
        }

        public Coupling CreateNewGearCoupling(Gear input, Gear output, string name = "")
        {
            Coupling coupling = couplingFactory.CreateNewGearCoupling(input, output, name);
            couplingMonitor.RegisterCoupling(coupling);
            return coupling;
        }

        public Coupling CreateNewLockedCoupling(Torqueable input, Torqueable output, string name = "")
        {
            Coupling coupling = couplingFactory.CreateNewLockedCoupling(input, output, name);
            couplingMonitor.RegisterCoupling(coupling);
            return coupling;
        }

        public BiPaddleCoupling CreateNewBiPaddleCoupling(Paddle paddle1, Paddle paddle2, string name = "")
        {
            BiPaddleCoupling coupling = couplingFactory.CreateNewBiPaddleCoupling(paddle1, paddle2, name);
            couplingMonitor.RegisterCoupling(coupling);
            return coupling;
        }

        public Coupling CreateNewOneWayCoupling(Gear input, Gear output, Direction direction, string name = "")
        {
            Coupling coupling = couplingFactory.CreateNewOneWayCoupling(input, output, direction, name);
            couplingMonitor.RegisterCoupling(coupling);
            return coupling;
        }

        public Coupling CreateNewOneWayPaddleCoupling(Gear input, Gear output, Direction direction, string name = "")
        {
            Coupling coupling = couplingFactory.CreateNewOneWayPaddleCoupling(input, output, direction, name);
            couplingMonitor.RegisterCoupling(coupling);
            return coupling;
        }

        public Coupling CreateNewFreeFlowingCoupling(Torqueable input, Torqueable output, string name = "")
        {
            Coupling coupling = couplingFactory.CreateNewFreeFlowingCoupling(input, output, name);
            couplingMonitor.RegisterCoupling(coupling);
            return coupling;
        }

        public Coupling CreateNewBidirectionalOpposingCoupling(Torqueable input, Torqueable output, string name = "")
        {
            Coupling coupling = couplingFactory.CreateNewBidirectionalOpposingCoupling(input, output, name);
            couplingMonitor.RegisterCoupling(coupling);
            return coupling;
        }

        public InitialStateCoupling CreateNewInitialStateCoupling(InitialState initialForce, Torqueable output, string name = "")
        {
            InitialStateCoupling coupling = couplingFactory.CreateNewInitialStateCoupling(initialForce, output, name);
            couplingMonitor.RegisterCoupling(coupling);
            return coupling;
        }
    }
}
