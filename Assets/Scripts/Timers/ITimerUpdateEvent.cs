using System;

namespace Timers
{
    public interface ITimerUpdateEvent
    {
        event Action<double> TimerUpdated;
    }
}