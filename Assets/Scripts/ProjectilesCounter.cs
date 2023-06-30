using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesCounter : MonoBehaviour
{
    [SerializeField] private ProjectilePoolAbstract[] _projectilePools;

    public int ProjectilesCount { get; private set; }

    private void OnValidate()
    {
        _projectilePools = FindObjectsOfType<ProjectilePoolAbstract>();
    }

    private void OnEnable()
    {
        foreach (var projectilePool in _projectilePools)
        {
            projectilePool.Got += OnGot;
            projectilePool.Removed += OnRemoved;
        }
    }

    private void OnDisable()
    {
        foreach (var projectilePool in _projectilePools)
        {
            projectilePool.Got -= OnGot;
            projectilePool.Removed -= OnRemoved;
        }
    }

    private void OnGot()
    {
        ProjectilesCount++;
    }

    private void OnRemoved()
    {
        ProjectilesCount--;
    }
}
