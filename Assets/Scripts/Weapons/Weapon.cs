using Configs;
using UnityEngine;
using UnityEngine.Events;

public class Weapon
{
    private readonly WeaponConfig _config;
    
    private Worm _worm;
    private float _currentShotPower = 0;
    private Transform _spawnPoint;
    private Transform _weaponViewTransform;

    private ProjectilePool ProjectilePool => _config.ProjectilePool;

    public bool IsShot { get; private set; } = false;

    public float CurrentShotPower => _currentShotPower;
    public WeaponConfig Config => _config;

    public event UnityAction<Projectile, Worm> ProjectileExploded;
    public event UnityAction<Projectile> Shot;
    public event UnityAction<float> ShotPowerChanged;
    public event UnityAction PointerLineEnabled;

    public Weapon(WeaponConfig config)
    {
        _config = config;
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

    public void Shoot()
    {
        if (IsShot)
            return;

        Projectile projectile = ProjectilePool.Pool.Get();
        projectile.Launch(_currentShotPower, _spawnPoint.position, _weaponViewTransform.right);
        projectile.Exploded += OnProjectileExploded;

        IsShot = true;
        Shot?.Invoke(projectile);
    }

    public void OnAssigned(Worm worm, Transform spawnPoint, Transform weaponViewTransform)
    {
        Reset();
        _worm = worm;
        _spawnPoint = spawnPoint;
        _weaponViewTransform = weaponViewTransform;
    }

    private void OnProjectileExploded(Projectile projectile)
    {
        projectile.Exploded -= OnProjectileExploded;
        ProjectileExploded?.Invoke(projectile, _worm);
    }
}
