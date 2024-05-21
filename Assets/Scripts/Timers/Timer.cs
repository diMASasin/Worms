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

        public event Action<float> TimerUpdated;

        public void Start(float interval, Action onElapsed)
        {
            _interval = interval;
            Reset();
            TimerUpdated?.Invoke(_timeLeft);
            CoroutinePerformer.StartCoroutine(StartTimer(onElapsed));
        }

        private IEnumerator StartTimer(Action onElapsed)
        {
            while (_timeLeft > 0)
            {
                yield return null;
                _timeLeft -= Time.deltaTime;
                TimerUpdated?.Invoke(_timeLeft);
            }

            _timeLeft = 0;
            Reset();
            onElapsed?.Invoke();
        }

        private void Reset()
        {
            _timeLeft = _interval;
        }
    }
}