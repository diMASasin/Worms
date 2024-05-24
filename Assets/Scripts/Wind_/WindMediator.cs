using System.Collections.Generic;
using Configs;
using Pools;
using Projectiles;
using UnityEngine;

namespace Wind_
{
    public class WindMediator
    {
        private readonly Wind _wind;

        private List<IProjectile> _projectilesUnderInfluence = new();

        public WindMediator(Wind wind)
        {
            _wind = wind;
        }

        public void FixedTick()
        {
            for (int i = 0; i < _projectilesUnderInfluence.Count; i++)
                _projectilesUnderInfluence[i].InfluenceOnVelocity(Vector2.right * (_wind.Velocity * Time.fixedDeltaTime));
        }

        public void InfluenceOnProjectileIfNecessary(IProjectile projectile)
        {
            if (projectile.Config.WindInfluence == true)
                _projectilesUnderInfluence.Add(projectile);
        }

        public void RemoveProjectileFromInfluence(IProjectile projectile)
        {
            if(_projectilesUnderInfluence.Contains(projectile))
                _projectilesUnderInfluence.Remove(projectile);
        }
    }
}