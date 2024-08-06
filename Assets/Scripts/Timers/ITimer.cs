using System;

namespace Timers
{
    public interface ITimer : IReadOnlyTimer
    {
        void Start(float interval, Action onElapsed);
        void Stop();
        void Resume();
        void Pause();
    }
}