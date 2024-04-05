using System;
using Pools;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectilePool", menuName = "ProjectilePool", order = 0)]
public class ProjectilePool : ScriptableObject
{
    [SerializeField] private ObjectPoolData<Projectile> _poolData = new();

    private ProjectileData _projectileData;

    public ObjectPool<Projectile> Pool { get; private set; }

    public static int Count { get; private set; }

    public void Init(Transform projectilesParent, ProjectileData projectileData)
    {
        _projectileData = projectileData;
        Pool = new ObjectPool<Projectile>(_poolData.Prefab, projectilesParent, _poolData.Amount);
        
        Pool.Removed += OnRemoved;
        Pool.Created += OnCreated;
        Pool.Got += OnGot;
    }

    private void OnDestroy()
    {
        Pool.Removed -= OnRemoved;
        Pool.Created -= OnCreated;
        Pool.Got -= OnGot;

        foreach (var projectile in Pool.Objects)
            projectile.Exploded -= OnExploded;

        Debug.Log("OnDestroy");
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

    protected void OnCreated(Projectile projectile)
    {
        projectile.Init(_projectileData);
        projectile.Exploded += OnExploded;
    }

    private void OnExploded(Projectile projectile)
    {
        Pool.Remove(projectile);
    }
}
