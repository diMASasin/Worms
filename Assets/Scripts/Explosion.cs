using System;
using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private ParticleSystem _explosionEffect;

    private float _explosionForce;
    private float _explosionUpwardsModifier;
    private int _damage;
    private float _projectileColliderRadius;
    private Action _particleSystemStopped;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Worm worm))
        {
            worm.AddExplosionForce(_explosionForce, transform.position, _explosionUpwardsModifier);
            worm.TakeDamage(CalculateDamage(_damage, worm.Collider2D));
        }
    }

    private int CalculateDamage(int maxDamage, Collider2D wormCollider)
    {
        float multiplier = 1 - (Vector3.Distance(transform.position, wormCollider.ClosestPoint(transform.position)) - _projectileColliderRadius);
        multiplier = Mathf.Clamp01(multiplier);
        if (multiplier >= 0.9f)
            multiplier = 1;

        return Convert.ToInt32(maxDamage * multiplier);
    }

    public void Explode(int damage, float projectileColliderRadius, float explosionForce, 
        float explosionUpwardsModifier, float explosionRadius, Action onParticleSystemStopped = null)
    {
        _projectileColliderRadius = projectileColliderRadius;
        _explosionForce = explosionForce;
        _explosionUpwardsModifier = explosionUpwardsModifier;
        _collider.radius = explosionRadius;
        _collider.enabled = true;
        _damage = damage;
        transform.parent = null;
        _explosionEffect.Play();
        _particleSystemStopped = onParticleSystemStopped;
        StartCoroutine(DelayedDisable());
    }

    private void OnParticleSystemStopped()
    {
        _particleSystemStopped?.Invoke();
    }

    private IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(0.1f);
        _collider.enabled = false;
    }
}
