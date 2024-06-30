using System;
using System.Collections;
using Cinemachine;
using Configs;
using UnityEngine;
using WormComponents;
using Zenject;

public class Explosion : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private ParticleSystem _explosionEffect;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    private float _explosionForce;
    private float _explosionUpwardsModifier;
    private int _damage;
    private IShovel _shovel;
    private ExplosionConfig _config;
    private CapsuleCollider2D _projectileCollider;

    public event Action<Explosion> Exploded;
    public event Action<Explosion> AnimationStopped;

    public void Init(IShovel shovel)
    {
        _shovel = shovel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Worm worm))
        {
            worm.AddExplosionForce(_explosionForce, transform.position, _explosionUpwardsModifier, _config.ExplosionRadius);
            worm.TakeDamage(CalculateDamage(_damage, worm));
        }
    }
    
    private int CalculateDamage(int maxDamage, Worm worm)
    {
        float multiplier = 1 - (Vector2.Distance(_projectileCollider.ClosestPoint(worm.transform.position), 
            worm.Collider2D.ClosestPoint(transform.position)));
        
        multiplier = Mathf.Clamp01(multiplier);
        
        if (multiplier >= 0.9f)
            multiplier = 1;

        return Convert.ToInt32(maxDamage * multiplier);
    }

    public void Explode(ExplosionConfig config, Vector3 newPosition, int damage, CapsuleCollider2D projectileCollider)
    {
        _projectileCollider = projectileCollider;
        _config = config;
        
        _explosionForce = config.ExplosionForce;
        _explosionUpwardsModifier = config.ExplosionUpwardsModifier;
        _collider.radius = config.ExplosionRadius;
        _damage = damage;

        _collider.enabled = true;

        transform.position= newPosition;
        _explosionEffect.Play();
        Vector3 impulseVelocity = new Vector3(Mathf.PerlinNoise1D(newPosition.x), Mathf.PerlinNoise1D(newPosition.y), 0);
        _impulseSource.GenerateImpulseAt(newPosition, impulseVelocity);
        _shovel.Dig(newPosition, config.LandDestroyRadius);

        Exploded?.Invoke(this);
        StartCoroutine(DelayedDisable());
    }

    private void OnParticleSystemStopped()
    {
        AnimationStopped?.Invoke(this);
    }

    private IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(0.1f);
        _collider.enabled = false;
    }
}
