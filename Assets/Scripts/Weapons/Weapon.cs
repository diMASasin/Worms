using System;
using Configs;
using Pools;
using Projectiles;
using UnityEngine;
using UnityEngine.Events;

public class Weapon
{
    private readonly WeaponConfig _config;
    private readonly Transform _spawnPosition;

    private float _currentShotPower = 0;
    private float _zRotation;
    private Projectile _projectile;

    public ProjectilePool ProjectilePool => _config.ProjectilePool;

    public bool IsShot { get; private set; } = false;

    public float CurrentShotPower => _currentShotPower;
    public WeaponConfig Config => _config;

    public event UnityAction<Projectile, Weapon> Shot;
    public event UnityAction<float> ShotPowerChanged;
    public event UnityAction PointerLineEnabled;
    public event Action<float> ScopeMoved;

    public Weapon(WeaponConfig config, Transform spawnPosition)
    {
        _config = config;
        _spawnPosition = spawnPosition;
    }

    public void Reset()
    {
        IsShot = false;
        _currentShotPower = 0;
    }

    public void MoveScope(float direction)
    {
        _zRotation = -direction * Config.ScopeSensetivity;
        //_zRotation = Mathf.Repeat(_zRotation, 720) - 360;
        ScopeMoved?.Invoke(_zRotation);
    }

    public void StartIncresePower()
    {
        if (IsShot)
            return;

        PointerLineEnabled?.Invoke();
    }

    public void IncreaseShotPower()
    {
        if (_currentShotPower >= _config.MaxShotPower || IsShot)
            return;

        _currentShotPower += _config.ShotPower * Time.deltaTime;

        if (_currentShotPower >= _config.MaxShotPower)
        {
            _currentShotPower = _config.MaxShotPower;
            Shoot();
        }

        ShotPowerChanged?.Invoke(_currentShotPower);
    }

    public void Reload(Projectile projectile)
    {
        _projectile = projectile;
    }
    
    public void Shoot()
    {
        if (IsShot || _projectile == null)
            return;

        _projectile.Launch(Vector2.one * _currentShotPower, _spawnPosition);
        _projectile = null;

        IsShot = true;
        _currentShotPower = 0;
        Shot?.Invoke(_projectile, this);
    }
}
