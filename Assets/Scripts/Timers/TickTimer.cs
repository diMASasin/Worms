using System;
using UnityEngine;

namespace Timers
{
    public class TickTimer : ITimer
    {
        private double _interval;
        private Coroutine _coroutine;
        private bool _paused;
        private Action _onElapsed;
        
        public double TimeLeft { get; private set; } = 0;
        public bool Started { get; private set; }

        public event Action<double> TimerUpdated;

        public void Start(float interval, Action onElapsed)
        {
            _onElapsed = onElapsed;
            _interval = interval;
            TimeLeft = _interval;
            Started = true;
            TimerUpdated?.Invoke(TimeLeft);
            
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Resume()
        {
            TimerUpdated?.Invoke(TimeLeft);
            _paused = false;
        }

        public void Pause() => _paused = true;

        public void Tick()
        {
            if (Started == false || _paused == true) return;

            Debug.Log($"({_interval}) Time Left {TimeLeft}");
            TimeLeft -= Time.deltaTime;
            TimerUpdated?.Invoke(TimeLeft);

            if (TimeLeft <= 0)
            {
                Reset();
                _onElapsed?.Invoke();
            }
        }

        private void Reset()
        {
            TimeLeft = _interval;
            Started = false;
        }
    }
}