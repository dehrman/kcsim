using System;
namespace KCSim.Parts.Mechanical
{
    public class PaddleFactory : IPaddleFactory
    {
        private readonly Func<IMotionTimer> motionTimerFactory;

        public PaddleFactory(Func<IMotionTimer> motionTimerFactory)
        {
            this.motionTimerFactory = motionTimerFactory;
        }

        public Paddle CreateNew(
            Paddle.Position initialPosition = Paddle.Position.Positive,
            string name = "default paddle name")
        {
            return new Paddle(
                motionTimer: motionTimerFactory.Invoke(),
                initialPosition: initialPosition,
                name: name);
        }
    }
}
