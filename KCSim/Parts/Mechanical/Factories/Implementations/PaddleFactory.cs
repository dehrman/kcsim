using System;
namespace KCSim.Parts.Mechanical
{
    public class PaddleFactory : IPaddleFactory
    {
        private readonly MotionTimerFactory motionTimerFactory;

        public PaddleFactory(MotionTimerFactory motionTimerFactory)
        {
            this.motionTimerFactory = motionTimerFactory;
        }

        public Paddle CreateNew(
            Paddle.Position initialPosition = Paddle.Position.Positive,
            string name = "default paddle name")
        {
            IMotionTimer motionTimer = motionTimerFactory.CreateNew();
            return new Paddle(
                motionTimer: motionTimer,
                initialPosition: initialPosition,
                name: name);
        }
    }
}
