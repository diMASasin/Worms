using System;
using System.Collections;
using Configs;
using UnityEngine;
using WormComponents;

public class Explosion : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private ParticleSystem _explosionEffect;

    private float _explosionForce;
    private float _explosionUpwardsModifier;
    private int _damage;
    private float _projectileColliderRadius;
    private ShovelWrapper _shovel;

    public event Action<Explosion> AnimationStopped;

    public void Init(ShovelWrapper shovel)
    {
        _shovel = shovel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Worm worm))
        {
            worm.AddExplosionForce(_explosionForce, transform.position, _explosionUpwardsModifier);
            worm.TakeDamage(CalculateDamage(_damage, worm.Collider2D));
        }
    }

    // private int CalculateDamage(int maxDamage, Collider2D wormCollider)
    // {
    //     float multiplier = 1 - (Vector3.Distance(transform.position, 
    //         wormCollider.ClosestPoint(transform.position)) - _projectileColliderRadius);
    //     multiplier = Mathf.Clamp01(multiplier);
    //     if (multiplier >= 0.9f)
    //         multiplier = 1;
    //
    //     return Convert.ToInt32(maxDamage * multiplier);
    // }
    
    private int CalculateDamage(int maxDamage, Collider2D wormCollider)
    {
        float multiplier = 1 - (Vector3.Distance(transform.position, 
            wormCollider.ClosestPoint(transform.position)));
        multiplier = Mathf.Clamp01(multiplier);
        if (multiplier >= 0.9f)
            multiplier = 1;

        return Convert.ToInt32(maxDamage * multiplier);
    }

    public void Explode(ExplosionConfig config, float projectileColliderRadius, Vector3 newPosition)
    {
        _projectileColliderRadius = projectileColliderRadius;
        _explosionForce = config.ExplosionForce;
        _explosionUpwardsModifier = config.ExplosionUpwardsModifier;
        _collider.radius = config.ExplosionRadius;
        _damage = config.Damage;

        _collider.enabled = true;
        transform.parent = null;

        transform.position = newPosition;
        _explosionEffect.Play();
        _shovel.Dig(transform.position, config.ExplosionRadius);

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
