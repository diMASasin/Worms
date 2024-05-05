using System;
using System.Collections.Generic;
using Projectiles;
using ScriptBoy.Digable2DTerrain;
using UnityEngine;
using Random = UnityEngine.Random;

public class FragmentsExplodeBehaviour : IExplodeBehaviour
{
    private readonly ProjectilePool _fragmentsPool;
    private readonly int _fragmentsAmount;
    private readonly Transform _spawnPoint;

    private readonly List<Projectile> _fragments = new();

    public event Action<List<Projectile>> FragmentsRecieved;

    public FragmentsExplodeBehaviour(ProjectilePool fragmentsPool, int fragmentsAmount, Transform spawnPoint)
    {
        _fragmentsPool = fragmentsPool;
        _fragmentsAmount = fragmentsAmount;
        _spawnPoint = spawnPoint;
    }

    public void OnExplode()
    {
        List<Projectile> projectiles = GetFragments();
        FragmentsRecieved?.Invoke(projectiles);
    }

    private List<Projectile> GetFragments()
    {
        for (int i = 0; i < _fragmentsAmount; i++)
        {
            var fragment = _fragmentsPool.Get();
            fragment.Launch(new Vector2(Random.Range(-2f, 2f), Random.Range(4f, 6f)));

            _fragments.Add(fragment);
        }

        return _fragments;
    }
}
