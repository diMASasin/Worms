using System;
using System.Collections.Generic;
using Configs;
using Pools;
using Projectiles;
using UnityEngine;
using Zenject;

namespace Wind_
{
    public class WindMediator : IDisposable, IFixedTickable
    {
        private Wind _wind;
        private IProjectileEvents _projectileEvents;

        private readonly List<Projectile> _projectilesUnderInfluence = new();

        public WindMediator(WindData data, WindView windView)
        {
            _wind = new Wind(data.MaxVelocity, data.Step);
            new WindEffect(_wind, data.Particles);
            windView.Init(_wind);
        }

        [Inject]
        private void Construct(IProjectileEvents projectileEvents)
        {
            _projectileEvents = projectileEvents;

            _projectileEvents.Launched += InfluenceOnProjectileIfNecessary;
            _projectileEvents.Exploded += RemoveProjectileFromInfluence;
        }

        public void Dispose()
        {
            _projectileEvents.Launched -= InfluenceOnProjectileIfNecessary;
            _projectileEvents.Exploded -= RemoveProjectileFromInfluence;
        }

        public void FixedTick()
        {
            for (int i = 0; i < _projectilesUnderInfluence.Count; i++)
                _projectilesUnderInfluence[i].InfluenceOnVelocity(Vector2.right * (_wind.Velocity * Time.fixedDeltaTime));
        }

        public void ChangeVelocity() => _wind.ChangeVelocity();

        private void InfluenceOnProjectileIfNecessary(Projectile projectile, Vector2 velocity)
        {
            if (projectile.Config.WindInfluence == true)
                _projectilesUnderInfluence.Add(projectile);
        }

        private void RemoveProjectileFromInfluence(Projectile projectile)
        {
            if(_projectilesUnderInfluence.Contains(projectile))
                _projectilesUnderInfluence.Remove(projectile);
        }
    }
}