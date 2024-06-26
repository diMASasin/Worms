using System;
using System.Collections.Generic;
using Projectiles;
using Projectiles.Behaviours.LaunchBehaviour;
using Timers;
using UI;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Object;

namespace Pools
{
    public class FollowingTimerViewPool : IPool<FollowingTimerView>, IDisposable
    {
        private readonly AllProjectilesEvents _projectileEvents;
        private readonly ObjectPool<FollowingTimerView> _followingTimerViewPool;
        private readonly Dictionary<Transform, FollowingTimerView> _timersTargets = new();

        public FollowingTimerViewPool(FollowingTimerView followingTimerViewPrefab, AllProjectilesEvents projectileEvents)
        {
            _projectileEvents = projectileEvents;
            
            _followingTimerViewPool = new ObjectPool<FollowingTimerView>(
                () => Instantiate(followingTimerViewPrefab),
                timer => timer.gameObject.SetActive(true),
                timer => timer.gameObject.SetActive(false));
            
            _projectileEvents.Launched += OnLaunched;
            _projectileEvents.Exploded += OnExploded;
        }

        public void Dispose()
        {
            Debug.Log($"{GetType().Name}");

            _projectileEvents.Launched -= OnLaunched;
            _projectileEvents.Exploded -= OnExploded;
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            if(projectile.TryGetComponent(out OnLaunchTimer timer) == false)
                return;
            
            FollowingTimerView timerView = Get();
            
            timerView.TimerView.Init(timer.Timer, TimerFormattingStyle.Seconds);
            timerView.FollowingObject.Follow(projectile.transform);
            _timersTargets.Add(projectile.transform, timerView);
        }

        private void OnExploded(Projectile projectile)
        {
            var target = projectile.transform;
            
            if(_timersTargets.ContainsKey(target) == false)
                return;
            
            Release(_timersTargets[target]);
            _timersTargets.Remove(target);
        }

        public FollowingTimerView Get() => _followingTimerViewPool.Get();

        public void Release(FollowingTimerView timerView) => _followingTimerViewPool.Release(timerView);
    }
}