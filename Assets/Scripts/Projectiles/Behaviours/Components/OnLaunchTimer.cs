using System;
using Infrastructure;
using Timers;
using UnityEngine;

namespace Projectiles.Behaviours.LaunchBehaviour
{
    public class OnLaunchTimer : MonoBehaviour, ICoroutinePerformer
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private float _interval;
        
        private Action _onElapsed;
        public Timer Timer;

        public void Awake() => Timer = new Timer(this);

        private void OnEnable()
        {
            _projectile.Launched += OnLaunched;
        }

        private void OnDisable()
        {
            _projectile.Launched -= OnLaunched;
        }

        private void OnLaunched(Projectile projectile, Vector2 vector2)
        {
            Timer.Start(_interval, OnTimerElapsed);
        }

        private void OnTimerElapsed()
        {
            _projectile.Explode();
        }
    }
}