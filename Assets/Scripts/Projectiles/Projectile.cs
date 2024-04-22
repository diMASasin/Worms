using System;
using Configs;
using DefaultNamespace;
using Pools;
using Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;
using Timer = Timers.Timer;

public class Projectile : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Animator _animator;

    private bool _dead;
    private Wind _wind;
    private readonly Timer _timer = new();
    private IProjectileExplodeModifier _explodeModifier;
    private ExplosionPool _explosionPool;
    private IProjectileLauchModifier[] _launchModifiers;

    public ProjectileConfig Config { get; private set; }
    public Vector2 Velocity { get; private set; }

    public Timer Timer => _timer;
    public Rigidbody2D Rigidbody2D => _rigidbody;
    public CircleCollider2D Collider2D => _collider;

    public event Action<Projectile> Exploded;
    public event Action LaunchedForward;
    public event Action LaunchedInDirection;
    public event Action<Quaternion> RotationChanged;

    public virtual void Init(Wind wind, ProjectileConfig config, ExplosionPool explosionPool,
        IProjectileExplodeModifier explodeModifier = null, IProjectileLauchModifier[] lauchModifiers = null) 
    {
        _wind = wind;
        Config = config;
        _collider.radius = Config.ColliderRadius;
        _explosionPool = explosionPool;
        _animator.runtimeAnimatorController = config.AnimatorController;

        _explodeModifier = explodeModifier;
        _launchModifiers = lauchModifiers;
    }

    public void Reset()
    {
        _rigidbody.velocity = Vector2.zero;
        Velocity = Vector2.zero;
        _dead = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Config.ExplodeOnKeyDown)
            Explode();
    }

    protected virtual void FixedUpdate()
    {
        if (Config.WindInfluence)
            InfluenceOnVelocityByWind();

        if (Config.LookInVelocityDirection)
            RotationChanged?.Invoke(Quaternion.LookRotation(Vector3.forward, _rigidbody.velocity));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Config.ExplodeOnCollision)
            Explode();
    }

    public void InfluenceOnVelocityByWind()
    {
        _rigidbody.velocity += new Vector2(_wind.Velocity * Time.fixedDeltaTime, 0);
    }
    
    public void Explode()
    {
        if (_dead) 
            return;

        _dead = true;

        var explosion = _explosionPool.Get();
        explosion.Explode(Config.ExplosionConfig, _collider.radius, transform.position);

        _explodeModifier?.OnExplode();
        Exploded?.Invoke(this);
    }

    public virtual void LaunchForward(float shotPower, Vector3 spawnPoint)
    {
        Launch(shotPower * transform.right, spawnPoint);
        LaunchedForward?.Invoke();
    }

    public virtual void LaunchInDirection(Vector2 direction, Vector3 spawnPoint)
    {
        Launch(direction, spawnPoint);
        LaunchedInDirection?.Invoke();
    }

    private void Launch(Vector2 velocity, Vector3 spawnPoint)
    {
        Reset();

        if (Config.IsExplodeWithDelay)
            _timer.Start(Config.ExplodeDelay, Explode);

        transform.position = spawnPoint;
        _rigidbody.velocity = velocity;

        foreach (var launchModifier in _launchModifiers)
            launchModifier?.OnLaunch(velocity);
    }
}