using System;
using System.Collections.Generic;
using Configs;
using Pools;
using Projectiles;
using UnityEngine;

public class WindMediator
{
    private readonly Wind _wind;
    private readonly IEnumerable<ProjectilePool> _pools;

    private List<Projectile> _projectilesUnderInfluence = new();

    public WindMediator(Wind wind, IEnumerable<ProjectilePool> pools)
    {
        _wind = wind;
        _pools = pools;
    }

    public void FixedTick()
    {
        for (int i = 0; i < _projectilesUnderInfluence.Count; i++)
            _projectilesUnderInfluence[i].InfluenceOnVelocity(Vector2.right * (_wind.Velocity * Time.fixedDeltaTime));
    }

    public void InfluenceOnProjectileIfNecessary(Projectile projectile, ProjectileConfig projectileConfig)
    {
        if (projectileConfig.WindInfluence == true)
            _projectilesUnderInfluence.Add(projectile);
    }

    public void RemoveProjectileFromInfluence(Projectile projectile)
    {
        if(_projectilesUnderInfluence.Contains(projectile))
            _projectilesUnderInfluence.Remove(projectile);
    }
}