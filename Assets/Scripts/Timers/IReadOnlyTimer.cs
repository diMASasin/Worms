using System;

namespace Timers
{
    public interface IReadOnlyTimer
    {
        public double TimeLeft { get; }
        bool Started { get; }
        event Action<double> TimerUpdated;
    }
}