using ScriptBoy.Digable2DTerrain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProjectilePoolAbstract : MonoBehaviour
{
    [SerializeField] protected int Amount;
    [SerializeField] protected ExplosionPool ExplosionPool;
    [SerializeField] protected Shovel Shovel;

    protected List<Projectile> Projectiles = new();
    protected List<Projectile> Used = new();

    private void OnValidate()
    {
        Shovel = FindObjectOfType<Shovel>();
    }

    public Projectile Get()
    {
        if (Used.Count == Projectiles.Count)
            CreateProjectile();

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
    }

    protected abstract Projectile CreateProjectile();
}
