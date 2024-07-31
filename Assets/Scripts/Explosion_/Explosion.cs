using System;
using System.Collections.Generic;
using Cinemachine;
using Configs;
using UnityEngine;
using WormComponents;

namespace Explosion_
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _explosionEffect;
        [SerializeField] private CinemachineImpulseSource _impulseSource;

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

        private int CalculateDamage(int maxDamage, float multiplier) => Convert.ToInt32(maxDamage * multiplier);

        private float CalculateMultiplier(Worm worm)
        {
            Vector3 projectileClosestPoint = _projectileCollider.ClosestPoint(worm.transform.position);
            Vector3 wormClosestPoint = worm.Collider2D.ClosestPoint(transform.position);
            Vector3 direction = projectileClosestPoint - wormClosestPoint;
            float distance = direction.magnitude;
            float multiplier = (_config.ExplosionRadius - distance) / _config.ExplosionRadius;
        
            multiplier = Mathf.Abs(multiplier);
            multiplier = Mathf.Clamp01(multiplier);
            
            return multiplier;
        }

        public void Explode(ExplosionConfig config, Vector3 newPosition, int damage, CapsuleCollider2D projectileCollider)
        {
            _config = config;
            _damage = damage;
            _projectileCollider = projectileCollider;
            _explosionUpwardsModifier = config.ExplosionUpwardsModifier;

            transform.position = newPosition;
            _explosionEffect.Play();
            _shovel.Dig(newPosition, config.LandDestroyRadius);
        
            var result = new List<Collider2D>();
            Physics2D.OverlapCircle(transform.position, _config.ExplosionRadius, _config.ContactFilter, result);
            ApplyExplosionToWorm(result);
        
            Exploded?.Invoke(this);
        }

        private void OnParticleSystemStopped() => AnimationStopped?.Invoke(this);
    }
}
