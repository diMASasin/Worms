using ScriptBoy.Digable2DTerrain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FragmentationGranadesPool : ProjectilePoolAbstract
{
    [SerializeField] private FragmentationGranadeProjectile _fragmentationGranadeTemplate;
    [SerializeField] private ProjectilesPool _fragmentsPool;

    private void Start()
    {
        for (int i = 0; i < Amount; i++)
            CreateFragmentationGranade();
    }

    private FragmentationGranadeProjectile CreateFragmentationGranade()
    {
        var granade = Instantiate(_fragmentationGranadeTemplate, transform);
        Projectiles.Add(granade);
        granade.Init(ExplosionPool, Shovel, _fragmentsPool, Wind);
        granade.gameObject.SetActive(false);
        return granade;
    }

    protected override Projectile CreateProjectile()
    {
        return CreateFragmentationGranade();
    }
}
