using System;
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
            _collider.radius = Config.ColliderRadius;
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(Config.ExplodeOnCollision)
                Explode();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && Config.ExplodeOnKeyDown)
                Explode();
        }

        private void FixedUpdate()
        {
            if (Config.LookInVelocityDirection)
                _spriteObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, _rigidbody.velocity);
            // if (_config.LookInVelocityDirection)
            //  _spriteObject.transform.right = _rigidbody.velocity;
        }

        public void ResetView()
        {
            _rigidbody.velocity = Vector2.zero;
        }

        public void Launch(Vector2 velocity)
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
    }
}