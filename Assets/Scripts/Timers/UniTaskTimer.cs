using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Timers
{
    public class UniTaskTimer : ITimer
    {
        private double _interval;
        private bool _paused;
        private bool _stopped;
        
        public double TimeLeft { get; private set; } = 0;
        public bool Started { get; private set; }
        
        public event Action<double> TimerUpdated;
        
        public void Start(float interval, Action onElapsed)
        {
            _interval = interval;
            Stop();
            Reset();
            Started = true;
            TimerUpdated?.Invoke(TimeLeft);
            
            StartTimer(onElapsed).Forget();
        }

        public void Stop() => _stopped = true;

        public void Resume()
        {
            TimeLeft = (int)TimeLeft;
            TimerUpdated?.Invoke(TimeLeft);
            _paused = false;
        }

        public void Pause() => _paused = true;

        private async UniTaskVoid StartTimer(Action onElapsed)
        {
            while (TimeLeft > 0)
            {
                await UniTask.Yield();
                
                if (_paused == true) continue;
                if(_stopped == true) return;

                TimeLeft -= Time.deltaTime;
                TimerUpdated?.Invoke(TimeLeft);
            }
            
            onElapsed?.Invoke();
        }

        private void Reset()
        {
            TimeLeft = _interval;
            Started = false;
            _paused = false;
            _stopped = false;
        }
    }
}
