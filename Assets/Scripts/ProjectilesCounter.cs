using System;
using UnityEngine;

public class ProjectilesCounter : IDisposable
{
    private readonly ProjectilePoolAbstract[] _projectilePools;

    public int ProjectilesCount { get; private set; }

    public ProjectilesCounter(params ProjectilePoolAbstract[] projectilePools)
    {
        _projectilePools = projectilePools;
        
        foreach (var projectilePool in _projectilePools)
        {
            projectilePool.Got += OnGot;
            projectilePool.Removed += OnRemoved;
        }
    }

    public void Dispose()
    {
        foreach (var projectilePool in _projectilePools)
        {
            projectilePool.Got -= OnGot;
            projectilePool.Removed -= OnRemoved;
        }
    }

    private void OnGot()
    {
        ProjectilesCount++;
    }

    private void OnRemoved()
    {
        ProjectilesCount--;
    }
}
