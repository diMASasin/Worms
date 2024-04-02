using ScriptBoy.Digable2DTerrain;
using UnityEngine;

public class FragmentationGranadesPool : ProjectilePoolAbstract
{
    private readonly Transform _projectilesParent;
    private readonly FragmentationGranadeProjectile _fragmentationGranadeTemplate;
    private readonly ProjectilesPool _fragmentsPool;

    public FragmentationGranadesPool(ExplosionPool explosionPool, Shovel shovel, Wind wind, int amount,
        Transform projectilesParent, FragmentationGranadeProjectile fragmentationGranadeTemplate, ProjectilesPool fragmentsPool) : base(explosionPool, shovel, wind, amount)
    {
        _projectilesParent = projectilesParent;
        _fragmentationGranadeTemplate = fragmentationGranadeTemplate;
        _fragmentsPool = fragmentsPool;

        for (int i = 0; i < Amount; i++)
            CreateFragmentationGranade();
    }

    private FragmentationGranadeProjectile CreateFragmentationGranade()
    {
        var granade = Object.Instantiate(_fragmentationGranadeTemplate, _projectilesParent);
        granade.Init(ExplosionPool, Shovel, _fragmentsPool, Wind);
        Projectiles.Add(granade);
        granade.gameObject.SetActive(false);
        return granade;
    }

    protected override Projectile CreateProjectile()
    {
        return CreateFragmentationGranade();
    }

}
