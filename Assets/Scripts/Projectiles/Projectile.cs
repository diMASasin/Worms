using Configs;
using DefaultNamespace;
using Pools;
using ScriptBoy.Digable2DTerrain;
using UnityEngine;
using UnityEngine.Events;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CircleCollider2D _collider2D;
    [SerializeField] private GameObject _spriteObject;

    [field: SerializeField] public ProjectileConfig ProjectileConfig { get; set; }

    private ShovelWrapper _shovel;
    private bool _dead;
    private ObjectPool<Explosion> _explosionPool;
    private Wind _wind;
    
    protected ProjectilePool FragmentsPool;

    public Rigidbody2D Rigidbody2D => _rigidbody;
    public Vector2 Velocity => _rigidbody.velocity;
    public GameObject SpriteRenderer => _spriteObject;

    public int Damage => ProjectileConfig.Damage;
    public float ExplosionForce => ProjectileConfig.ExplosionForce;
    public float ExplosionRadius => ProjectileConfig.ExplosionRadius;
    public float ExplosionUpwardsModifier => ProjectileConfig.ExplosionUpwardsModifier;

    public event UnityAction<Projectile> Exploded;

    public virtual void Init(ProjectileData projectileData) 
    {
        _shovel = projectileData.Shovel;
        _explosionPool = projectileData.ExplosionsPool;
        _wind = projectileData.Wind;
        FragmentsPool = projectileData.FragmentsPool;
    }

    protected virtual void FixedUpdate()
    {
        if(ProjectileConfig.WindInfluence)
            _rigidbody.velocity += new Vector2(_wind.Velocity * Time.fixedDeltaTime, 0);
    }

    public void Reset()
    {
        _dead = false;
        _rigidbody.velocity = Vector2.zero;
    }

    public virtual void Launch(float currentShotPower, Vector3 spawnPoint, Vector3 direction)
    {
        Reset();

        transform.position = spawnPoint;
        SetVelocity(currentShotPower, direction);
    }
    
    public void Explode()
    {
        if (_dead) 
            return;

        _dead = true;
        var position = transform.position;

        _shovel.Dig(position, ProjectileConfig.ExplosionRadius);

        var explosion = _explosionPool.Get();
        explosion.transform.position = position;

        explosion.Explode(Damage, _collider2D.radius, ExplosionForce, ExplosionUpwardsModifier, ExplosionRadius,
            () => _explosionPool.Remove(explosion));

        Exploded?.Invoke(this);
    }

    private void SetVelocity(float currentShotPower, Vector3 direction)
    {
        _rigidbody.velocity = currentShotPower * direction;
        Debug.Log($"{_rigidbody.velocity} {currentShotPower} {direction}");
    }
}
