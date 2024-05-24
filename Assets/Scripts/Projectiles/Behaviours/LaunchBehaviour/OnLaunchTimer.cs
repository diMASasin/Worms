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
        private readonly IFollowingTimerViewPool _followingTimerViewPool;
        private readonly Projectile _projectile;
        private readonly float _interval;
        private readonly Action _onElapsed;
        private FollowingTimerView _followingTimerView;

        public readonly Timer Timer = new();

        public OnLaunchTimer(IFollowingTimerViewPool followingTimerViewPool, Projectile projectile, 
            float interval, Action onElapsed)
        {
            _followingTimerViewPool = followingTimerViewPool;
            _projectile = projectile;
            _interval = interval;
            _onElapsed = onElapsed;
            
        }

        public void OnLaunch(Vector2 velocity)
        {
            Timer.Start(_interval, OnTimerElapsed);
            _followingTimerView = _followingTimerViewPool.Get();
            _followingTimerView.TimerView.Init(Timer, TimerFormattingStyle.Seconds);
            _followingTimerView.FollowingObject.Connect(_projectile.transform);
            
            _projectile.Exploded += OnExploded;
        }

        private void OnExploded(Projectile projectile)
        {
            _projectile.Exploded -= OnExploded;
            
            if (_followingTimerView != null)
            {
                _followingTimerView.FollowingObject.Disonnect();
                _followingTimerViewPool.Release(_followingTimerView);
                _followingTimerView = null;
            }
        }

        private void OnTimerElapsed()
        {
            _onElapsed?.Invoke();
        }
    }
}