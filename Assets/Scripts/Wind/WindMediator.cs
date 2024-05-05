using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;

public class WindMediator : IDisposable
{
    private readonly Wind _wind;
    private readonly Game _game;
    private readonly IEnumerable<ProjectilePool> _pools;

    private List<Projectile> _projectilesUnderInfluence = new();

    public WindMediator(Wind wind, Game game, IEnumerable<ProjectilePool> pools)
    {
        _wind = wind;
        _game = game;
        _pools = pools;

        foreach (var pool in _pools)
            pool.Got += OnProjectileLaunched;

        _game.TurnStarted += OnTurnStarted;
    }

    public void FixedTick()
    {
        for (int i = 0; i < _projectilesUnderInfluence.Count; i++)
        {
            _projectilesUnderInfluence[i].InfluenceOnVelocity(Vector2.right * _wind.Velocity);
        }
    }

    public void Dispose()
    {
        foreach (var pool in _pools)
            pool.Got -= OnProjectileLaunched;

        _game.TurnStarted -= OnTurnStarted;
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

    private void OnTurnStarted(Worm worm, Team team)
    {
        _wind.ChangeVelocity();
    }
}