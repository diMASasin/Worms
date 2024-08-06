using System;
using Infrastructure.Interfaces;
using Timers;
using UnityEngine;

namespace Projectiles.Behaviours.Components
{
    public class OnLaunchTimer : MonoBehaviour, ICoroutinePerformer
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private float _interval;
        
        public ReactiveTimer Timer { get; private set; } = new();
        
        private Action _onElapsed;

        private void OnEnable() => _projectile.Launched += OnLaunched;

        private void OnDisable() => _projectile.Launched -= OnLaunched;

        private void OnLaunched(Projectile projectile, Vector2 vector2)
        {
            Timer.Start(_interval, () => _projectile.Explode());
        }
    }
}