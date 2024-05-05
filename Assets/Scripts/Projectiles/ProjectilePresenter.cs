using System;
using System.Collections.Generic;
using Configs;
using Pools;
using Timers;
using UnityEngine;

namespace Projectiles
{
    public class ProjectilePresenter : IDisposable
    {
        public ProjectileView View { get; private set; }
        public Projectile Projectile { get; private set; }

        private readonly Timer _timer = new();
        private List<ILaunchBehaviour> _launchBehaviours = new();
        private List<IExplodeBehaviour> _explodeBehaviours = new();

        public readonly ProjectileConfig Config;

        public ProjectilePresenter(Projectile projectile, ProjectileView view, ProjectileConfig config)
        {
            Config = config;
            Projectile = projectile;
            View = view;

            View.CollidedWithMapBound += OnCollidedWithMapBound;
            View.CollisionEnter += OnCollisionEnter;

            Projectile.Launched += OnLaunched;
            Projectile.Exploded += OnExploded;

            _timer.TimerUpdated += OnTimerUpdated;
        }

        private void OnExploded(Projectile projectile)
        {
            View.Explode(projectile);
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Space) && Config.ExplodeOnKeyDown)
                Projectile.Explode();

            _timer.Tick();
        }

        public void Dispose()
        {
            View.CollidedWithMapBound -= OnCollidedWithMapBound;
            View.CollisionEnter -= OnCollisionEnter;

            Projectile.Launched -= OnLaunched;
            Projectile.Exploded -= OnExploded;

            _timer.TimerUpdated -= OnTimerUpdated;
        }

        public void AddLaunchBehaviour(ILaunchBehaviour behaviour)
        {
            _launchBehaviours.Add(behaviour);
        }

        public void AddExplodeBehaviour(IExplodeBehaviour behaviour)
        {
            _explodeBehaviours.Add(behaviour);
        }

        private void OnTimerUpdated(float time)
        {
            View.UpdateText(time);
        }

        private void OnCollidedWithMapBound()
        {
            Projectile.Explode();
        }

        private void OnCollisionEnter(Collision2D collision2D)
        {
            if (Config.ExplodeOnCollision)
                Projectile.Explode();
        }

        private void OnLaunched(Vector2 velocity)
        {
            if (Config.IsExplodeWithDelay)
                _timer.Start(Config.ExplodeDelay, () => Projectile.Explode());
        }
    }
}