using ScriptBoy.Digable2DTerrain;
using UnityEngine;

public class ProjectilesPool : ProjectilePoolAbstract
{
    private readonly Projectile _projectileTemplate;
    private readonly Transform _projectilesParent;

    public ProjectilesPool(ExplosionPool explosionPool, Shovel shovel, Wind wind, int amount,
        Projectile projectileTemplate, Transform projectilesParent) : base(explosionPool, shovel, wind, amount)
    {
        _projectileTemplate = projectileTemplate;
        _projectilesParent = projectilesParent;

        for (int i = 0; i < Amount; i++)
            CreateProjectile();
    }

    protected override Projectile CreateProjectile()
    {
        var projectile = Object.Instantiate(_projectileTemplate, _projectilesParent);
        projectile.Init(ExplosionPool, Shovel, Wind);
        Projectiles.Add(projectile);
        projectile.gameObject.SetActive(false);
        return projectile;
    }
}
