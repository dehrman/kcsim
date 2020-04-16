using System;
using System.Collections.Generic;
using Xunit;
using System.Text;
using KCSim;
using KCSim.Parts.Mechanical;
using Moq;

namespace KCSimTests
{
    public class TestUtil
    {
        public static Mock<IMotionTimer> GetMockMotionTimer()
        {
            Mock<IMotionTimer> mockMotionTimer = new Mock<IMotionTimer>();

            // Set up the motion timer such that the delegate is invoked as soon as the timer is started.
            mockMotionTimer.Setup(x => x.Start(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<IMotionTimer.OnTimerCompletionDelegate>()))
                .Callback<double, double, IMotionTimer.OnTimerCompletionDelegate>(
                    (d, v, onTimerCompletionDelegate) => onTimerCompletionDelegate.Invoke(v));

            return mockMotionTimer;
        }

        public static Mock<IPaddleFactory> GetMockPaddleFactory()
        {
            return GetMockPaddleFactory(GetMockMotionTimer());
        }

        public static Mock<IPaddleFactory> GetMockPaddleFactory(Mock<IMotionTimer> mockMotionTimer)
        {
            Mock<IPaddleFactory> mockPaddleFactory = new Mock<IPaddleFactory>();
            Paddle paddle = new Paddle(mockMotionTimer.Object);
            mockPaddleFactory.Setup(x => x.CreateNew(It.IsAny<Paddle.Position>(), It.IsAny<string>())).Returns(paddle);
            return mockPaddleFactory;
        }

        public static Mock<IRelayFactory> GetMockRelayFactory()
        {
            Mock<IRelayFactory> mockRelayFactory = new Mock<IRelayFactory>();
            Mock<IPaddleFactory> mockPaddleFactory = GetMockPaddleFactory();
            // mockRelayFactory.Setup(x => x.CreateNew(It.Is<bool>(x => true), It.Is<bool>(x => true))
            mockRelayFactory.Setup(x => x.CreateNew(It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns<bool, bool>((isControlPositive, isInputPositive) =>
                    new Relay(mockPaddleFactory.Object, isControlPositive, isInputPositive)
                );
            return mockRelayFactory;
        }
    }
}
