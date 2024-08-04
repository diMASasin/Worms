using System;

namespace Timers
{
    public interface ITimer : ITimerUpdateEvent
    {
        bool Started { get; }
        void Start(float interval, Action onElapsed);
        void Stop();
        void Resume();
        void Pause();
    }
}