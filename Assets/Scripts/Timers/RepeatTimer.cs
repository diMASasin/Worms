using System;
using System.Collections;
using Infrastructure;
using UnityEngine;

namespace Timers
{
    public class RepeatTimer
    {
        private readonly Timer _timer = new();
        private bool _shouldRepeat;

        public void Repeat(float interval, Action onElapsed)
        {
            _timer.Start(interval, () =>
            {
                onElapsed?.Invoke();
                _timer.Start(interval, onElapsed);
            });
        }

        public void Stop() => _timer.Stop();
    }
}