using System;
using System.Collections.Generic;
using Projectiles;
using UnityEngine;
using Zenject;

namespace Wind_
{
    public class WindMediator : IDisposable, IFixedTickable
    {
        public readonly Wind Wind;
        private readonly IProjectileEvents _projectileEvents;

        private readonly List<Projectile> _projectilesUnderInfluence = new();

        public WindMediator(WindData data, WindView windView, IProjectileEvents projectileEvents)
        {
            _projectileEvents = projectileEvents;

            Wind = new Wind(data.MaxVelocity, data.Step);
            new WindEffect(Wind, data.Particles);
            windView.Init(Wind);
            
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
                _projectilesUnderInfluence[i].InfluenceOnVelocity(Vector2.right * (Wind.Velocity * Time.fixedDeltaTime));
        }

        public void ChangeVelocity() => Wind.ChangeVelocity();

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