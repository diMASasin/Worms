using ScriptBoy.Digable2DTerrain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentationGranadeProjectile : GranadeProjectile
{
    [SerializeField] private int _fragmentsAmount;
    
    private ProjectilesPool _fragmentsPool;

    public void Init(ExplosionPool explosionPool, Shovel shovel, ProjectilesPool fragmentsPool, Wind wind)
    {
        base.Init(explosionPool, shovel, wind);
        _fragmentsPool = fragmentsPool;
    }

    protected override void OnTimerOut()
    {
        base.OnTimerOut();
        for (int i = 0; i < _fragmentsAmount; i++)
        {
            var fragment = _fragmentsPool.Get();
            fragment.transform.position = transform.position;
            fragment.gameObject.SetActive(true);
            fragment.Rigidbody2D.velocity += new Vector2(Random.Range(-2f, 2f), Random.Range(4f, 6f));
            fragment.Exploded += Remove;
        }
    }

    private void Remove(Projectile projectile)
    {
        projectile.Exploded -= Remove;
        _fragmentsPool.Remove(projectile);
    }
}
