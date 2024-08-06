using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Timers
{
    public class ReactiveTimer : IDisposable
    {
        private readonly ReactiveProperty<double> _timeLeft = new();
        
        private double _interval;
        private bool _paused;
        private bool _stopped;

        public bool Started { get; private set; }

        public ReadOnlyReactiveProperty<double> TimeLeft => _timeLeft;

        public void Dispose() => _timeLeft.Dispose();

        public void Start(float interval, Action onElapsed)
        {
            _interval = interval;
            Stop();
            Reset();
            Started = true;
            
            StartTimer(onElapsed).Forget();
        }

        public void Stop() => _stopped = true;

        public void Resume()
        {
            _timeLeft.Value = (int)_timeLeft.Value;
            _paused = false;
        }

        public void Pause() => _paused = true;

        private async UniTaskVoid StartTimer(Action onElapsed)
        {
            while (_timeLeft.Value > 0)
            {
                await UniTask.Yield();
                
                if (_paused == true) continue;
                if(_stopped == true) return;

                _timeLeft.Value -= Time.deltaTime;
            }
            
            onElapsed?.Invoke();
        }

        private void Reset()
        {
            _timeLeft.Value = _interval;
            Started = false;
            _paused = false;
            _stopped = false;
        }
    }
}