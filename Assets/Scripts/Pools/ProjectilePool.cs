using System;
using Configs;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "ProjectilePool", menuName = "ProjectilePool", order = 0)]
public class ProjectilePool : ScriptableObject
{
    [SerializeField] private ProjectileConfig _config;
    [SerializeField] private int _amount;

    private ObjectPool<Projectile> _pool;

    public ProjectileConfig Config => _config;

    public event Action<Projectile, ProjectileConfig> Got;
    public event Action Released;

    public static int Count { get; private set; }

    public void Init()
    {
        _pool = new ObjectPool<Projectile>(OnCreated, OnGet, OnRelease, OnDestroy, true, _amount);
    }

    private void OnDestroy()
    {
        _pool.Dispose();
    }

    public Projectile Get() => _pool.Get();

    private void OnGet(Projectile projectile)
    {
        projectile.Reset();
        Count++;
        Got?.Invoke(projectile, _config);
    }

    private void OnRelease(Projectile projectile)
    {
        Count--;
        Released?.Invoke();
    }

    private Projectile OnCreated()
    {
        Projectile projectile = new();
        projectile.Exploded += OnExploded;
        return projectile;
    }

    private void OnExploded(Projectile projectile)
    {
        _pool.Release(projectile);
    }

    private void OnDestroy(Projectile projectile)
    {
        projectile.Exploded -= OnExploded;
    }
}
