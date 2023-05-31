using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectilesPool : MonoBehaviour
{
    [SerializeField] private Projectile _projectileTemplate;
    [SerializeField] private int _amount;

    private List<Projectile> _projectiles = new();
    private List<Projectile> _used = new();

    private void Start()
    {
        for (int i = 0; i < _amount; i++)
        {
            var projectile = CreateProjectile();
            projectile.gameObject.SetActive(false);
        }
    }

    public Projectile Get()
    {
        if (_used.Count == _projectiles.Count)
            CreateProjectile();

        var projectile = _projectiles.First(projectile => !_used.Contains(projectile));

        _used.Add(projectile);
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void Remove(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _used.Remove(projectile);
    }

    private Projectile CreateProjectile()
    {
        var projectile = Instantiate(_projectileTemplate, transform);
        _projectiles.Add(projectile);
        return projectile;
    }
}
