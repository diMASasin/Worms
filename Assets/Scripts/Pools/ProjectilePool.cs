using System;
using Configs;
using Factories;
using Pools;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "ProjectilePool", menuName = "ProjectilePool", order = 0)]
public class ProjectilePool : ScriptableObject
{
    [SerializeField] private ProjectileConfig _config;
    [SerializeField] private int _amount;

    private ProjectileFactory _factory;

    public ObjectPool<Projectile> Pool { get; private set; }

    public static int Count { get; private set; }

    public void Init(ProjectileFactory factory)
    {
        _factory = factory;

        Pool = new ObjectPool<Projectile>(OnCreated, OnGot, OnRemoved, OnDestroy, true, _amount);
    }

    private void OnDestroy()
    {
        Pool.Dispose();
    }

    private void OnGot(Projectile projectile)
    {
        Count++;
    }

    public void OnRemoved(Projectile projectile)
    {
        projectile.Reset();
        Count--;
    }

    private Projectile OnCreated()
    {
        Projectile projectile = _factory.GetProjectile(_config);
        projectile.Exploded += OnExploded;
        return projectile;
    }

    private void OnExploded(Projectile projectile)
    {
        Pool.Release(projectile);
    }

    private void OnDestroy(Projectile projectile)
    {
        projectile.Exploded -= OnExploded;
    }
}
