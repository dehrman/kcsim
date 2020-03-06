using System;
namespace KCSim
{
    public interface IMotionTimer
    {
        public delegate void OnTimerCompletionDelegate(double velocity);
        public void Start(double degreesToDestination, double velocity, OnTimerCompletionDelegate onTimerCompletion);
        public void Stop();
    }
}
