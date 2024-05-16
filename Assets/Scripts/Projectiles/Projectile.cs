using System;
using Configs;
using Timers;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private GameObject _spriteObject;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private CircleCollider2D _collider;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _animator;

        private ProjectileConfig _config;
        private bool _dead;

        public CircleCollider2D Collider => _collider;
        public Rigidbody2D Rigidbody => _rigidbody;
        
        private event Action Reseted;
        public event Action<Projectile> Exploded;
        public event Action<Projectile, Vector2> Launched;

        public void Init(ProjectileConfig config)
        {
            _config = config;
            _spriteRenderer.sprite = _config.Sprite;
            _collider.radius = _config.ColliderRadius;
            _animator.runtimeAnimatorController = _config.AnimatorController;
        }
        
        public void ResetProjectile()
        {
            Reseted?.Invoke();
            _rigidbody.velocity = Vector2.zero;
            _dead = false;
        }
        
        public void InfluenceOnVelocity(Vector2 additionalVelocity)
        {
            _rigidbody.velocity += additionalVelocity;
         }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(_config.ExplodeOnCollision)
                Explode();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _config.ExplodeOnKeyDown)
                Explode();
        }

        private void FixedUpdate()
        {
            if (_config.LookInVelocityDirection)
                _spriteObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, _rigidbody.velocity);
            // if (_config.LookInVelocityDirection)
            //  _spriteObject.transform.right = _rigidbody.velocity;
        }

        public void ResetView()
        {
            _rigidbody.velocity = Vector2.zero;
        }

        public void Launch(Vector2 shotPower, Transform spawnPoint)
        {
            _rigidbody.AddForce(shotPower * (spawnPoint.right), ForceMode2D.Impulse);
            
            Launched?.Invoke(this, shotPower);
        }

        public void Explode()
        {
            if (_dead) 
                return;

            _dead = true;
            Exploded?.Invoke(this);
        }
    }
}