using System;
using Timers;
using UnityEngine;

namespace Projectiles.Behaviours.LaunchBehaviour
{
    public class OnLaunchTimer : ILaunchBehaviour
    {
        private readonly float _interval;
        private readonly Action _onElapsed;

        private Timer _timer;

        public OnLaunchTimer(float interval, Action onElapsed)
        {
            _interval = interval;
            _onElapsed = onElapsed;
        }

        public void OnLaunch(Vector2 velocity)
        {
            _timer.Start(_interval, OnTimerElapsed);
        }

        private void OnTimerElapsed()
        {
            _onElapsed?.Invoke();
        }
    }
}