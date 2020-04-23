﻿using System;
using KCSim;
using KCSim.Parts.Logical;
using KCSim.Parts.Mechanical;
using KCSim.Parts.Mechanical.Machines;
using KCSim.Parts.State;
using KCSim.Physics;
using KCSim.Physics.Couplings;
using Moq;
using Xunit;

namespace KCSimTests
{
    public class TestUtil
    {
        private ICouplingService couplingService = null;
        private ICouplingMonitor couplingMonitor = null;
        private IGateMonitor gateMonitor = null;
        private IStateMonitor stateMonitor = null;

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

        public IGateMonitor GetSingletonGateMonitor()
        {
            if (gateMonitor == null)
            {
                gateMonitor = new GateMonitor();
            }
            return gateMonitor;
        }

        public IStateMonitor GetSingletonStateMonitor()
        {
            if (stateMonitor == null)
            {
                stateMonitor = new StateMonitor(GetSingletonCouplingMonitor());
            }
            return stateMonitor;
        }

        public IGateFactory GetGateFactory()
        {
            return new GateFactory(
                couplingService: GetSingletonCouplingService(),
                bidirectionalLatchFactory: GetMockBidirectionalLatchFactory().Object,
                gateMonitor: GetSingletonGateMonitor());
        }

        public IStateFactory GetStateFactory()
        {
            return new StateFactory(
                couplingService: GetSingletonCouplingService(),
                gateFactory: GetGateFactory(),
                stateMonitor: GetSingletonStateMonitor());
        }

        /**
         * Return a new version of this force in the same direction but at the desired level.
         * 
         * Should only be used for testing purposes.
         */
        public static Force ToLevel(Force forceToChange, Force forceWithDesiredLevel)
        {
            if (forceToChange.Equals(Force.ZeroForce))
            {
                return Force.ZeroForce;
            }
            double sign = forceToChange.Velocity / Math.Abs(forceToChange.Velocity);
            return new Force(Math.Abs(forceWithDesiredLevel.Velocity) * sign);
        }

        /**
         * Return a new version of this force in the same direction but at the desired level.
         * 
         * Should only be used for testing purposes.
         */
        public static Force ToLevel(double forceToChange, double desiredLevel)
        {
            return ToLevel(new Force(forceToChange), new Force(desiredLevel));
        }

        public static void AssertDirectionsEqual(Force force1, Force force2)
        {
            var releveledForce1 = ToLevel(force1, force2);
            Assert.Equal(releveledForce1, force2);
        }
    }
}
