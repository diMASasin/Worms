using Configs;
using ScriptBoy.Digable2DTerrain;
using UnityEngine;
using UnityEngine.Events;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CircleCollider2D _collider2D;
    [SerializeField] private GameObject _spriteObject;

    [field: SerializeField] public ProjectileConfig ProjectileConfig { get; set; }

    private Shovel _shovel;
    private bool _dead;
    private ExplosionPool _explosionPool;
    private Wind _wind;

    public Rigidbody2D Rigidbody2D => _rigidbody;
    public Vector2 Velocity => _rigidbody.velocity;
    public GameObject SpriteRenderer => _spriteObject;

    public event UnityAction<Projectile> Exploded;

    public virtual void Init(ExplosionPool explosionPool, Shovel shovel, Wind wind) 
    {
        _explosionPool = explosionPool;
        _shovel = shovel;
        _wind = wind;
    }

    protected virtual void FixedUpdate()
    {
        if(ProjectileConfig.WindInfluence)
            _rigidbody.velocity += new Vector2(_wind.Velocity * Time.fixedDeltaTime, 0);
    }

    public void SetVelocity(float currentShotPower)
    {
        _rigidbody.velocity = currentShotPower * transform.right;
    }

    public void Reset()
    {
        _dead = false;
    }

    public void ResetVelocity()
    {
        _rigidbody.velocity = Vector2.zero;
    }

    public abstract void OnShot();

    public void Explode()
    {
        if (_dead) 
            return;

        _dead = true;
        _shovel.radius = ProjectileConfig.ExplosionRadius;
        _shovel.transform.position = transform.position;
        _shovel.Dig();
        var explosion = _explosionPool.Get();
        explosion.transform.position = transform.position;
        explosion.Explode(ProjectileConfig.Damage, _collider2D.radius, ProjectileConfig.ExplosionForce, 
            ProjectileConfig.ExplosionUpwardsModifier, ProjectileConfig.ExplosionRadius, () => _explosionPool.Remove(explosion));
        Exploded?.Invoke(this);
    }
}
