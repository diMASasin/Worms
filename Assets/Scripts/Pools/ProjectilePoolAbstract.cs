using ScriptBoy.Digable2DTerrain;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProjectilePoolAbstract
{
    protected int Amount;
    protected ExplosionPool ExplosionPool;
    protected Shovel Shovel;
    protected Wind Wind;

    protected List<Projectile> Projectiles = new();
    protected List<Projectile> Used = new();

    public event Action Got;
    public event Action Removed;

    public ProjectilePoolAbstract(ExplosionPool explosionPool, Shovel shovel, Wind wind, int amount)
    {
        ExplosionPool = explosionPool;
        Shovel = shovel;
        Wind = wind;
        Amount = amount;
    }

    public Projectile Get()
    {
        if (Used.Count == Projectiles.Count)
            CreateProjectile();

        Got?.Invoke();
        var projectile = Projectiles.First(projectile => !Used.Contains(projectile));

        Used.Add(projectile);
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void Remove(Projectile projectile)
    {
        projectile.Reset();
        projectile.gameObject.SetActive(false);
        Used.Remove(projectile);
        Removed?.Invoke();
    }

    protected abstract Projectile CreateProjectile();
}
