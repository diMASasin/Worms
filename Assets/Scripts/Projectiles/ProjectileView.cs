using System;
using Configs;
using DefaultNamespace;
using Pools;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Projectiles
{
    public class ProjectileView : MonoBehaviour
    {
        [SerializeField] private GameObject _spriteObject;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private CircleCollider2D _collider;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private FollowingObject _followingCanvas;

        private ExplosionPool _explosionPool;

        public CircleCollider2D Collider => _collider;
        public Rigidbody2D Rigidbody => _rigidbody;

        private ProjectileConfig _config;

        public event Action<ProjectileView> Exploded;
        public event Action CollidedWithMapBound;
        public event Action<Collision2D> CollisionEnter;

        public void Init(ProjectileConfig config, ExplosionPool explosionPool)
        {
            _config = config;
            _spriteRenderer.sprite = _config.Sprite;
            _explosionPool = explosionPool;
            _collider.radius = _config.ColliderRadius;
            _animator.runtimeAnimatorController = _config.AnimatorController;

            _followingCanvas.gameObject.SetActive(_config.IsExplodeWithDelay);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionEnter?.Invoke(collision);
        }

        public void ResetView()
        {
            _rigidbody.velocity = Vector2.zero;
        }

        public void SetPosition(Transform spawnPoint)
        {
            transform.position = spawnPoint.position;
            transform.right = spawnPoint.transform.right;
        }

        public void OnCollidedWithMapBound()
        {
            CollidedWithMapBound?.Invoke();
        }

        private void FixedUpdate()
        {
            if (_config.LookInVelocityDirection)
                _spriteObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, _rigidbody.velocity);
        }

        public void OnLaunched(Vector2 shotPower)
        {
            _followingCanvas.Disonnect();
            _rigidbody.AddForce(shotPower * transform.right, ForceMode2D.Impulse);
        }

        public void UpdateText(float timeLeft)
        {
            _text.text = timeLeft.ToString();
        }

        public void Explode(Projectile projectile)
        {
            _followingCanvas.Connect();
            
            var explosion = _explosionPool.Get();
            explosion.Explode(_config.ExplosionConfig, _collider.radius, transform.position);

            Exploded?.Invoke(this);
        }
    }
}