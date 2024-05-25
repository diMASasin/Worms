using Configs;
using Projectiles;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Pools
{
    public class ExplosionPool
    {
        [SerializeField] private ExplosionConfig _config;
        [SerializeField] private int _amount;

        private Transform _objectsParent;
        private IShovel _shovelWrapper;

        private ObjectPool<Explosion> _pool;
        private IProjectileEvents _projectileEvents;

        public static int Count { get; private set; }

        public ExplosionPool (Transform objectsParent, IShovel shovelWrapper, IProjectileEvents projectileEvents)
        {
            _projectileEvents = projectileEvents;
            _objectsParent = objectsParent;
            _shovelWrapper = shovelWrapper;
            
            _pool = new ObjectPool<Explosion>(CreateObject, OnGet, OnRelease, OnExplosionDestroy, defaultCapacity: _amount);
            
            _projectileEvents.Launched += OnLaunched;
        }

        private void OnLaunched(Projectile arg1, Vector2 arg2)
        {
            _projectileEvents.Exploded += OnExploded;
        }

        private void OnExploded(Projectile projectile)
        {
            _projectileEvents.Launched -= OnLaunched;
            _projectileEvents.Exploded -= OnExploded;
            
            Explosion explosion = Get();
            ExplosionConfig explosionConfig = projectile.Config.ExplosionConfig;
            explosion.Explode(explosionConfig, projectile.Collider.radius, projectile.transform.position);
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        public Explosion Get() => _pool.Get();

        private Explosion CreateObject()
        {
            var explosion = Object.Instantiate(_config.Prefab, _objectsParent);
            explosion.Init(_shovelWrapper);
            explosion.AnimationStopped += OnAnimationStopped;

            explosion.gameObject.SetActive(false);
            return explosion;
        }

        private void OnAnimationStopped(Explosion explosion)
        {
            _pool.Release(explosion);
        }

        private void OnExplosionDestroy(Explosion explosion)
        {
            explosion.AnimationStopped -= OnAnimationStopped;
        }

        private void OnGet(Explosion explosion)
        {
            explosion.gameObject.SetActive(true);
            Count++;
        }

        private void OnRelease(Explosion explosion)
        {
            explosion.gameObject.SetActive(false);
            Count--;
        }
    }
}