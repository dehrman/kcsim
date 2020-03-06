using System;
using System.Timers;
using static KCSim.IMotionTimer;

namespace KCSim
{
    public class MotionTimer : IMotionTimer
    {
        // For the unit velocity (that is velocity = 1), how many degrees are swept per millisecond?
        // If this value is 10, that means 10 degrees are swept per millisecond.
        private static readonly double UnitVelocityInDegreesPerMillisecond = 1;

        private OnTimerCompletionDelegate onTimerCompletion;
        private double velocity = 0;

#nullable enable
        private Timer? timer;
#nullable disable

        public void Start(double degreesToDestination, double velocity, OnTimerCompletionDelegate onTimerCompletion)
        {
            if (timer != null)
            {
                throw new InvalidOperationException("Must call Stop on pre-existing timer before calling Start");
            }

            this.onTimerCompletion = onTimerCompletion;

            // Store the velocity for later access; we'll need to pass it to the delegate that gets fired when the
            // timer expires as it may be used by the caller for additional computation. Why isn't this just the
            // responsibility of the caller? On the off-chance that the velocity changed after this timer was started,
            // it's better to report the source of truth that the timer sees than to leave it to the caller to
            // mistakenly assume that that the timer elapsed in a duration as computed by its new velocity.
            this.velocity = velocity;

            double remainingMillisecondsToDestination =
                degreesToDestination / (Math.Abs(velocity) * UnitVelocityInDegreesPerMillisecond);

            timer = new Timer(remainingMillisecondsToDestination);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer?.Stop();
            timer?.Dispose();
            timer = null;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Stop();
            onTimerCompletion(velocity);
        }
    }
}
