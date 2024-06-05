using System;
using System.Collections.Generic;
using Configs;
using Timers;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        [SerializeField] private GameObject _spriteObject;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private CircleCollider2D _collider;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _animator;

        public ProjectileConfig Config { get; private set; }

        private bool _dead;

        public CircleCollider2D Collider => _collider;
        public Rigidbody2D Rigidbody => _rigidbody;
        
        public event Action<Projectile> Exploded;
        public event Action<Projectile, Vector2> Launched;

        public void Init(ProjectileConfig config)
        {
            Config = config;
            _spriteRenderer.sprite = Config.Sprite;
            _animator.runtimeAnimatorController = Config.AnimatorController;
        }
        
        public void ResetProjectile()
        {
            _rigidbody.velocity = Vector2.zero;
            _dead = false;
        }
        
        public void InfluenceOnVelocity(Vector2 additionalVelocity)
        {
            _rigidbody.velocity += additionalVelocity;
        }

        public void ResetView()
        {
            _rigidbody.velocity = Vector2.zero;
        }

        public virtual void Launch(Vector2 velocity)
        {
            _rigidbody.AddForce(velocity, ForceMode2D.Impulse);
            
            Launched?.Invoke(this, velocity);
        }

        public void Explode()
        {
            if (_dead) 
                return;

            _dead = true;
            Exploded?.Invoke(this);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && Config.ExplodeOnKeyDown)
                Explode();
        }

        private void FixedUpdate()
        {
            if (Config.LookInVelocityDirection)
                _spriteObject.transform.up = _rigidbody.velocity;
        }

        private void LateUpdate()
        {
            GetCollisionDetectionParameters(out Vector2 origin, out float radius);
            List<RaycastHit2D> results = new();
            
            int count = Physics2D.CircleCast(origin, radius, Vector2.zero, Config.ContactFilter, results, distance: 0);
            
            if (Config.ExplodeOnCollision && count > 0) Explode();
        }

        private void OnDrawGizmos()
        {
            GetCollisionDetectionParameters(out Vector2 origin, out float radius);
            Gizmos.DrawSphere(origin, radius);
        }

        private void GetCollisionDetectionParameters(out Vector2 origin, out float radius)
        {
            origin = transform.position;
            radius = _collider.radius * 1.2f;
        }
    }
}