using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float _explosionForce = 2f;
    [SerializeField] private float _explosionUpwardsModifier = 2f;
    [SerializeField] private int _damage;
    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private ParticleSystem _explosionEffect;

    private float _bombColliderRadius;
    private Action _particleSystemStopped;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Worm worm))
        {
            worm.AddExplosionForce(_explosionForce, transform, _explosionUpwardsModifier);
            worm.TakeDamage(CalculateDamage(_damage, worm.Collider2D));
        }
    }

    private int CalculateDamage(int maxDamage, Collider2D wormCollider)
    {
        float multiplier = 1 - (Vector3.Distance(transform.position, wormCollider.ClosestPoint(transform.position)) - _bombColliderRadius);
        multiplier = Mathf.Clamp01(multiplier);
        if (multiplier >= 0.9f)
            multiplier = 1;

        //Debug.Log("1 - Distance: " + (1 - (Vector3.Distance(transform.position, 
            //wormCollider.ClosestPoint(transform.position)) - _bombColliderRadius)) + " " + maxDamage * multiplier);
        return Convert.ToInt32(maxDamage * multiplier);
    }

    public void Explode(float bombColliderRadius, Action onParticleSystemStopped = null)
    {
        _bombColliderRadius = bombColliderRadius;
        _collider.enabled = true;
        transform.parent = null;
        _explosionEffect.Play();
        _particleSystemStopped = onParticleSystemStopped;
        StartCoroutine(DelayedDisable());
    }

    private void OnParticleSystemStopped()
    {
        Debug.Log("OnParticleSystemStopped");
        _particleSystemStopped?.Invoke();
    }

    private IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(0.1f);
        _collider.enabled = false;
    }
}
