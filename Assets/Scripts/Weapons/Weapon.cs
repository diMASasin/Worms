using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Renderer _pointerRenderer;
    [SerializeField] private float _sencetivity = 0.01f;
    [SerializeField] private float _scopeSencetivity = 0.7f;
    [SerializeField] private float _speedMultiplier = 0.03f;
    [SerializeField] private Projectile _bombPrefab;
    [SerializeField] private Transform _pointerLine;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _shotPower = 5;
    [SerializeField] private float _maxShotPower = 5;
    [SerializeField] private ProjectilesPool _projectilesPool;
    [SerializeField] private ExplosionPool _explosionPool;
    
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

    public void RaiseScope()
    {
        transform.Rotate(new Vector3(0, 0, -_scopeSencetivity) * Time.deltaTime);
    }

    public void LowerScope()
    {
        transform.Rotate(new Vector3(0, 0, _scopeSencetivity) * Time.deltaTime);
    }

    public void IncreaseShotPower()
    {
        if (_currentShotPower >= _maxShotPower || IsShot)
            return;

        _currentShotPower += _shotPower * Time.deltaTime;
        _pointerLine.localScale = new Vector3(_currentShotPower / _maxShotPower, 1, 1);

        if (_currentShotPower >= _maxShotPower)
        {
            _currentShotPower = _maxShotPower;
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
        projectile.Init(velocity, _explosionPool);
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
