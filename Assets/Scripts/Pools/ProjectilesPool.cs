using UnityEngine;

public class ProjectilesPool : ProjectilePoolAbstract
{
    [SerializeField] private Projectile _projectileTemplate;

    private void Start()
    {
        for (int i = 0; i < Amount; i++)
            CreateProjectile();
    }

    protected override Projectile CreateProjectile()
    {
        var projectile = Instantiate(_projectileTemplate, transform);
        projectile.Init(ExplosionPool, Shovel, Wind);
        Projectiles.Add(projectile);
        projectile.gameObject.SetActive(false);
        return projectile;
    }
}
