using System;
using System.Collections;
using Infrastructure;
using UnityEngine;
using Zenject;

namespace Timers
{
    public class Timer : ITimerUpdateEvent
    {
        private double _interval;
        private double _timeLeft = 0;
        private Coroutine _coroutine;
        private bool _paused;
        private ICoroutinePerformer _coroutinePerformer;
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
            Started = true;
            TimerUpdated?.Invoke(_timeLeft);
            _coroutine = _coroutinePerformer.StartCoroutine(StartTimer(onElapsed));
        }

        public void Stop()
        {
            if(_coroutine != null)
                _coroutinePerformer.StopCoroutine(_coroutine);
            
            Reset();
        }

        public void Resume()
        {
            _timeLeft = (int)_timeLeft;
            TimerUpdated?.Invoke(_timeLeft);
            _paused = false;
        }

        public void Pause() => _paused = true;

        private IEnumerator StartTimer(Action onElapsed)
        {
            while (_timeLeft > 0)
            {
                yield return null;
                
                if (_paused == true) continue;

                _timeLeft -= Time.deltaTime;
                TimerUpdated?.Invoke(_timeLeft);
            }

            Stop();
            onElapsed?.Invoke();
        }

        private void Reset()
        {
            _timeLeft = _interval;
            Started = false;
        }
    }
}