using ScriptBoy.Digable2DTerrain;
using System.Collections;
using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Renderer _pointerRenderer;
    [SerializeField] private Transform _pointerLine;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private ProjectilePoolAbstract _projectilesPool;
    [SerializeField] private WeaponConfig _config;
    
    private Worm _worm;
    private Vector3 _mouseStart;
    private float _currentShotPower = 0;

    public bool IsShot { get; private set; } = false;

    public float CurrentShotPower => _currentShotPower;

    public event UnityAction<Projectile, Worm> ProjectileExploded;
    public event UnityAction<Projectile> Shot;

    private void Start()
    {
        _pointerRenderer.enabled = false;
    }

    public void Reset()
    {
        IsShot = false;
        _currentShotPower = 0;
    }

    public void EnablePointerLine()
    {
        if (IsShot)
            return;

        _pointerRenderer.enabled = true;
        _mouseStart = Input.mousePosition;
    }

    public void MoveScope(float direction)
    {
        transform.Rotate(new Vector3(0, 0, direction * _config.ScopeSensetivity) * Time.deltaTime);
    }

    public void IncreaseShotPower()
    {
        if (_currentShotPower >= _config.MaxShotPower || IsShot)
            return;

        _currentShotPower += _config.ShotPower * Time.deltaTime;
        _pointerLine.localScale = new Vector3(_currentShotPower / _config.MaxShotPower, 1, 1);

        if (_currentShotPower >= _config.MaxShotPower)
        {
            _currentShotPower = _config.MaxShotPower;
            Shoot();
        }
    }

    public void Shoot()
    {
        if (IsShot)
            return;

        Vector3 delta = Input.mousePosition - _mouseStart;
        Vector3 velocity = _currentShotPower * transform.right;

        _pointerRenderer.enabled = false;
        Projectile projectile = _projectilesPool.Get();
        projectile.transform.position = _spawnPoint.position;
        projectile.Reset();
        Shot?.Invoke(projectile);
        IsShot = true;
        projectile.Exploded += OnProjectileExploded;
        projectile.SetVelocity(velocity);
        projectile.OnShot();
        gameObject.SetActive(false);
    }

    public void SetWorm(Worm worm)
    {
        _worm = worm;
    }

    private void OnProjectileExploded(Projectile projectile)
    {
        projectile.Exploded -= OnProjectileExploded;
        ProjectileExploded?.Invoke(projectile, _worm);
        _projectilesPool.Remove(projectile);
    }
}
