using System;
using System.Collections;
using Infrastructure.Interfaces;
using UnityEngine;

namespace Timers
{
    public class Timer : ITimer
    {
        private double _interval;
        private Coroutine _coroutine;
        private bool _paused;
        private readonly ICoroutinePerformer _coroutinePerformer;
        
        public double TimeLeft { get; private set; } = 0;
        public bool Started { get; private set; }
        
        public event Action<double> TimerUpdated;

        public Timer(ICoroutinePerformer coroutinePerformer)
        {
            _coroutinePerformer = coroutinePerformer;
        }
        
        public void Start(float interval, Action onElapsed)
        {
            _interval = interval;
            Stop();
            Reset();
            Started = true;
            TimerUpdated?.Invoke(TimeLeft);
            _coroutine = _coroutinePerformer.StartCoroutine(StartTimer(onElapsed));
        }

        public void Stop()
        {
            if(_coroutine != null)
                _coroutinePerformer.StopCoroutine(_coroutine);
        }

        public void Resume()
        {
            TimeLeft = (int)TimeLeft;
            TimerUpdated?.Invoke(TimeLeft);
            _paused = false;
        }

        public void Pause() => _paused = true;

        private IEnumerator StartTimer(Action onElapsed)
        {
            while (TimeLeft > 0)
            {
                yield return null;
                
                if (_paused == true) continue;

                TimeLeft -= Time.deltaTime;
                TimerUpdated?.Invoke(TimeLeft);
            }

            Stop();
            onElapsed?.Invoke();
        }

        private void Reset()
        {
            TimeLeft = _interval;
            Started = false;
        }
    }
}