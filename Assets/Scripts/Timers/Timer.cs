using System;
using UnityEngine;

namespace Timers
{
    public class Timer
    {
        private float _interval;
        private float _timeLeft;
        private bool _started = false;

        private event Action OnElapsedAction;

        public event Action<float> TimerUpdated;

        public void SetInterval(float interval)
        {
            _interval = interval;
        }

        public void Start(float interval, Action onElapsed)
        {
            SetInterval(interval);
            Reset();
            _started = true;

            TimerUpdated?.Invoke(_timeLeft);
            OnElapsedAction = onElapsed;
        }

        public void Stop()
        {
            _started = false;
        }

        public void Continue()
        {
            _started = true;
        }

        public void Tick()
        {
            if (!_started) return;
        
            _timeLeft -= Time.deltaTime;

            if(_timeLeft <= 0)
            {
                _timeLeft = 0;
                Stop();

                OnElapsedAction?.Invoke();
            }

            TimerUpdated?.Invoke(_timeLeft);
        }

        private void Reset()
        {
            _timeLeft = _interval;
            _started = false;
        } 
    }
}