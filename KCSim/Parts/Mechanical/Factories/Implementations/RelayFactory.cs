using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical
{
    public class RelayFactory : IRelayFactory
    {
        private readonly ICouplingService couplingService;
        private readonly IPaddleFactory paddleFactory;

        public RelayFactory(
            ICouplingService couplingService,
            IPaddleFactory paddleFactory)
        {
            this.couplingService = couplingService;
            this.paddleFactory = paddleFactory;
        }

        public Relay CreateNew(
            Direction enableDirection,
            Direction inputDirection,
            Relay.InitialState initialState = Relay.InitialState.Disabled,
            string name = "")
        {
            return new Relay(
                couplingService,
                paddleFactory,
                enableDirection,
                inputDirection,
                initialState,
                name);
        }
    }
}
