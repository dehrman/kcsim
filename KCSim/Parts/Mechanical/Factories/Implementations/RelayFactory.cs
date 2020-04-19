using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;

namespace KCSim.Parts.Mechanical
{
    class RelayFactory : IRelayFactory
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

        public Relay CreateNew(Direction enableDirection, Direction inputDirection, string name = "")
        {
            return new Relay(
                couplingService: couplingService,
                paddleFactory: paddleFactory,
                enableDirection: enableDirection,
                inputDirection: enableDirection,
                name: name);
        }
    }
}
