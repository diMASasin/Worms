using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Configs;
using UnityEngine;
using WormComponents;
using Zenject;
using Random = UnityEngine.Random;

public class Explosion : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffect;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _cameraShakeFactor = 0.1f;

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
        // ApplyExplosionToWorm(collision);
    }

    private void ApplyExplosionToWorm(IEnumerable<Collider2D> collisions)
    {
        foreach (var collision in collisions)
        {
            if (collision.gameObject.TryGetComponent(out Worm worm))
            {
                float multiplier = CalculateMultiplier(worm);
                Vector3 direction = worm.transform.position - transform.position;

                worm.AddExplosionForce(direction, _config.ExplosionForce, multiplier, _explosionUpwardsModifier);
                worm.TakeDamage(CalculateDamage(_damage, multiplier));
            }
        }
    }

    private int CalculateDamage(int maxDamage, float multiplier)
    {
        // if (multiplier >= 0.8f)
        //     multiplier = 1;
        return Convert.ToInt32(maxDamage * multiplier);
    }

    private float CalculateMultiplier(Worm worm)
    {
        Vector3 projectileClosestPoint = _projectileCollider.ClosestPoint(worm.transform.position);
        Vector3 wormClosestPoint = worm.Collider2D.ClosestPoint(transform.position);
        Vector3 direction = projectileClosestPoint - wormClosestPoint;
        float distance = direction.magnitude;
        float multiplier = (_config.ExplosionRadius - distance) / _config.ExplosionRadius;
        
        multiplier = Mathf.Abs(multiplier);
        return multiplier;
    }

    public void Explode(ExplosionConfig config, Vector3 newPosition, int damage, CapsuleCollider2D projectileCollider)
    {
        _projectileCollider = projectileCollider;
        _config = config;
        
        _explosionUpwardsModifier = config.ExplosionUpwardsModifier;
        _damage = damage;

        transform.position= newPosition;
        _explosionEffect.Play();
        Vector3 impulseVelocity = new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f));
        _impulseSource.GenerateImpulseAt(newPosition, impulseVelocity * _cameraShakeFactor);
        _shovel.Dig(newPosition, config.LandDestroyRadius);
        
        var result = new List<Collider2D>();
        var count = Physics2D.OverlapCircle(transform.position, _config.ExplosionRadius, _config.ContactFilter, result);
        ApplyExplosionToWorm(result);
        
        Exploded?.Invoke(this);
    }

    private void OnParticleSystemStopped()
    {
        AnimationStopped?.Invoke(this);
    }
}
