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
        var obj = Instantiate(_fragmentationGranadeTemplate, transform);
        Projectiles.Add(obj);
        obj.Init(ExplosionPool, Shovel, _fragmentsPool, Wind);
        obj.gameObject.SetActive(false);
        return obj;
    }

    protected override Projectile CreateProjectile()
    {
        return CreateFragmentationGranade();
    }
}
