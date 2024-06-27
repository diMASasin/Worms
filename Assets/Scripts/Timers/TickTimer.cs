using System;
using UnityEngine;

namespace Timers
{
    public class TickTimer : ITimerUpdateEvent
    {
        private double _interval;
        private double _timeLeft = 0;
        private Coroutine _coroutine;
        private bool _paused;
        private Action _onElapsed;
        
        [field: SerializeField] public bool Started { get; private set; }

        public event Action<double> TimerUpdated;

        public void Start(float interval, Action onElapsed)
        {
            _onElapsed = onElapsed;
            _interval = interval;
            _timeLeft = _interval;
            Started = true;
            TimerUpdated?.Invoke(_timeLeft);
            
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Resume()
        {
            TimerUpdated?.Invoke(_timeLeft);
            _paused = false;
        }

        public void Pause() => _paused = true;

        public void Tick()
        {
            if (Started == false || _paused == true) return;

            Debug.Log($"({_interval}) Time Left {_timeLeft}");
            _timeLeft -= Time.deltaTime;
            TimerUpdated?.Invoke(_timeLeft);

            if (_timeLeft <= 0)
            {
                Reset();
                _onElapsed?.Invoke();
            }
        }

        private void Reset()
        {
            _timeLeft = _interval;
            Started = false;
        }
    }
}