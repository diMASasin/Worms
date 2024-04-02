using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionPool
{
    private readonly Explosion _explosionTemplate;
    private readonly Transform _projectilesParent;

    private readonly List<Explosion> _explosions = new();
    private readonly List<Explosion> _used = new();

    public ExplosionPool(Explosion explosionTemplate, Transform projectilesParent, int amount)
    {
        _explosionTemplate = explosionTemplate;
        _projectilesParent = projectilesParent;

        for (int i = 0; i < amount; i++)
            CreateExplosion();
    }

    public Explosion Get()
    {
        if (_used.Count == _explosions.Count)
            CreateExplosion();

        var explosion = _explosions.First(explosion => !_used.Contains(explosion));

        _used.Add(explosion);
        explosion.gameObject.SetActive(true);
        return explosion;
    }

    public void Remove(Explosion explosion)
    {
        explosion.transform.parent = _projectilesParent;
        _used.Remove(explosion);
    }

    private void CreateExplosion()
    {
        var explosion = Object.Instantiate(_explosionTemplate, _projectilesParent);
        _explosions.Add(explosion);
    }
}
