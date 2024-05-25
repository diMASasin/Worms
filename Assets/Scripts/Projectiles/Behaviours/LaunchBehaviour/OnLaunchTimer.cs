using System;
using Pools;
using Timers;
using UI;
using UnityEngine;
using UnityEngine.Pool;

namespace Projectiles.Behaviours.LaunchBehaviour
{
    public class OnLaunchTimer : ILaunchBehaviour
    {
        private readonly IPool<FollowingTimerView> _pool;
        private readonly Projectile _projectile;
        private readonly float _interval;
        private readonly Action _onElapsed;
        private readonly Timer _timer = new();

        private FollowingTimerView _followingTimerView;

        public OnLaunchTimer(IPool<FollowingTimerView> pool, Projectile projectile, float interval, Action onElapsed)
        {
            _pool = pool;
            _projectile = projectile;
            _interval = interval;
            _onElapsed = onElapsed;
        }

        public void OnLaunch(Vector2 velocity)
        {
            _timer.Start(_interval, OnTimerElapsed);
            _followingTimerView = _pool.Get();
            _followingTimerView.TimerView.Init(_timer, TimerFormattingStyle.Seconds);
            _followingTimerView.FollowingObject.Connect(_projectile.transform);
            
            _projectile.Exploded += OnExploded;
        }

        private void OnExploded(Projectile projectile)
        {
            _projectile.Exploded -= OnExploded;
            
            if (_followingTimerView != null)
            {
                _followingTimerView.FollowingObject.Disonnect();
                _pool.Release(_followingTimerView);
                _followingTimerView = null;
            }
        }

        private void OnTimerElapsed()
        {
            _onElapsed?.Invoke();
        }
    }
}