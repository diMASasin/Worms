using System;
using Pools;
using Timers;
using UI;
using UnityEngine;
using UnityEngine.Pool;

namespace Projectiles.Behaviours.LaunchBehaviour
{
    public class OnLaunchTimer : MonoBehaviour
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private float _interval;
        
        private Action _onElapsed;
        public Timer Timer;

        private void Start()
        {
            Timer = new Timer();
        }

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