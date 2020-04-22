using KCSim;
using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using Moq;

namespace KCSimTests
{
    public class TestUtil
    {
        private ICouplingMonitor couplingMonitor = null;
        private ICouplingService couplingService = null;

        public ICouplingMonitor GetSingletonCouplingMonitor()
        {
            if (couplingMonitor == null)
            {
                couplingMonitor = new CouplingMonitor(new PartsGraph());
            }
            return couplingMonitor;
        }
        
        public ICouplingService GetSingletonCouplingService()
        {
            if (couplingService == null)
            {
                CouplingFactory couplingFactory = new CouplingFactory();
                couplingService = new CouplingService(GetSingletonCouplingMonitor(), couplingFactory);
            }
            return couplingService;
        }

        public Mock<IMotionTimer> GetMockMotionTimer()
        {
            Mock<IMotionTimer> mockMotionTimer = new Mock<IMotionTimer>();

            // Set up the motion timer such that the delegate is invoked as soon as the timer is started.
            mockMotionTimer.Setup(x => x.Start(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<IMotionTimer.OnTimerCompletionDelegate>()))
                .Callback<double, double, IMotionTimer.OnTimerCompletionDelegate>(
                    (d, v, onTimerCompletionDelegate) => onTimerCompletionDelegate.Invoke(v));

            return mockMotionTimer;
        }

        public Mock<IPaddleFactory> GetMockPaddleFactory()
        {
            return GetMockPaddleFactory(GetMockMotionTimer());
        }

        public Mock<IPaddleFactory> GetMockPaddleFactory(Mock<IMotionTimer> mockMotionTimer)
        {
            Mock<IPaddleFactory> mockPaddleFactory = new Mock<IPaddleFactory>();
            Paddle paddle = new Paddle(mockMotionTimer.Object, Paddle.Position.Positive, "test paddle");
            mockPaddleFactory.Setup(x => x.CreateNew(It.IsAny<Paddle.Position>(), It.IsAny<string>())).Returns(paddle);
            return mockPaddleFactory;
        }

        public Mock<IRelayFactory> GetMockRelayFactory()
        {
            Mock<IRelayFactory> mockRelayFactory = new Mock<IRelayFactory>();
            Mock<IPaddleFactory> mockPaddleFactory = GetMockPaddleFactory();
            mockRelayFactory.Setup(x => x.CreateNew(It.IsAny<Direction>(), It.IsAny<Direction>(), It.IsAny<string>()))
                .Returns<Direction, Direction, string>((enableDirection, inputDirection, name) =>
                    new Relay(GetSingletonCouplingService(), mockPaddleFactory.Object, enableDirection, inputDirection, name)
                );
            return mockRelayFactory;
        }

        public Mock<IBidirectionalLatchFactory> GetMockBidirectionalLatchFactory()
        {
            Mock<IBidirectionalLatchFactory> mockBidirectionalLatchFactory = new Mock<IBidirectionalLatchFactory>();
            mockBidirectionalLatchFactory.Setup(x => x.CreateNew(It.IsAny<string>()))
                .Returns<string>((name) => new BidirectionalLatch(GetSingletonCouplingService(), GetMockRelayFactory().Object, name: name));
            return mockBidirectionalLatchFactory;
        }
    }
}
