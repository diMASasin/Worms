using System;
using System.Collections.Generic;
using Configs;
using Pools;
using Projectiles;
using UnityEngine;

public class WindMediator : IDisposable
{
    private readonly Wind _wind;
    private readonly IEnumerable<ProjectilePool> _pools;

    private List<Projectile> _projectilesUnderInfluence = new();

    public WindMediator(Wind wind, IEnumerable<ProjectilePool> pools)
    {
        _wind = wind;
        _pools = pools;

        foreach (var pool in _pools)
            pool.Got += OnProjectileLaunched;
    }

    public void FixedTick()
    {
        for (int i = 0; i < _projectilesUnderInfluence.Count; i++)
        {
            _projectilesUnderInfluence[i].InfluenceOnVelocity(Vector2.right * (_wind.Velocity * Time.fixedDeltaTime));
        }
    }

    public void Dispose()
    {
        foreach (var pool in _pools)
            pool.Got -= OnProjectileLaunched;
    }

    private void OnProjectileLaunched(Projectile projectile, ProjectileConfig projectileConfig)
    {
        if (projectileConfig.WindInfluence == true)
            _projectilesUnderInfluence.Add(projectile);

        projectile.Exploded += OnExploded;
    }

    private void OnExploded(Projectile projectile)
    {
        projectile.Exploded -= OnExploded;

        if(_projectilesUnderInfluence.Contains(projectile))
            _projectilesUnderInfluence.Remove(projectile);
    }
}