using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Timers
{
    public class UniTaskTimer : ITimer
    {
        private double _interval;
        private double _timeLeft = 0;
        private bool _paused;
        private bool _stopped;
        public bool Started { get; private set; }
        
        public event Action<double> TimerUpdated;
        
        public void Start(float interval, Action onElapsed)
        {
            _interval = interval;
            Stop();
            Reset();
            Started = true;
            TimerUpdated?.Invoke(_timeLeft);
            
            StartTimer(onElapsed).Forget();
        }

        public void Stop() => _stopped = true;

        public void Resume()
        {
            _timeLeft = (int)_timeLeft;
            TimerUpdated?.Invoke(_timeLeft);
            _paused = false;
        }

        public void Pause() => _paused = true;

        private async UniTaskVoid StartTimer(Action onElapsed)
        {
            while (_timeLeft > 0)
            {
                await UniTask.Yield();
                
                if (_paused == true) continue;
                if(_stopped == true) return;

                _timeLeft -= Time.deltaTime;
                TimerUpdated?.Invoke(_timeLeft);
            }
            
            onElapsed?.Invoke();
        }

        private void Reset()
        {
            _timeLeft = _interval;
            Started = false;
            _paused = false;
            _stopped = false;
        }
    }
}
