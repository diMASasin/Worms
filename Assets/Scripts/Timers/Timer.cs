using System;
using System.Collections;
using Infrastructure;
using UnityEngine;

namespace Timers
{
    public class Timer
    {
        private float _interval;
        private float _timeLeft = 0;
        private Coroutine _coroutine;
        
        public event Action<float> TimerUpdated;

        public void Start(float interval, Action onElapsed)
        {
            _interval = interval;
            Stop();
            TimerUpdated?.Invoke(_timeLeft);
            _coroutine = CoroutinePerformer.StartCoroutine(StartTimer(onElapsed));
        }

        public void Stop()
        {
            if(_coroutine != null)
                CoroutinePerformer.StopCoroutine(_coroutine);
            Reset();
        }

        private IEnumerator StartTimer(Action onElapsed)
        {
            while (_timeLeft > 0)
            {
                yield return null;
                _timeLeft -= Time.deltaTime;
                TimerUpdated?.Invoke(_timeLeft);
            }

            Reset();
            onElapsed?.Invoke();
        }

        private void Reset()
        {
            _timeLeft = 0;
            _timeLeft = _interval;
        }
    }
}