using System;
using System.Collections.Generic;
using Configs;
using Pools;
using Projectiles;
using UnityEngine;

namespace Wind_
{
    public class WindMediator : IDisposable
    {
        private readonly Wind _wind;
        private readonly IProjectileEvents _projectileEvents;

        private readonly List<IProjectile> _projectilesUnderInfluence = new();
        private WindEffect _windEffect;

        public WindMediator(WindData data, WindView windView, IProjectileEvents projectileEvents)
        {
            _wind = new Wind(data.MaxVelocity, data.Step);
            _windEffect = new WindEffect(_wind, data.Particles);
            windView.Init(_wind);

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

        private void RemoveProjectileFromInfluence(IProjectile projectile)
        {
            if(_projectilesUnderInfluence.Contains(projectile))
                _projectilesUnderInfluence.Remove(projectile);
        }
    }
}