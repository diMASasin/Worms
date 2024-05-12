using Configs;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Pools
{
    [CreateAssetMenu(fileName = "ExplosionPool", menuName = "ExplosionPool", order = 0)]
    public class ExplosionPool : ScriptableObject
    {
        [SerializeField] private ExplosionConfig _config;
        [SerializeField] private int _amount;

        private Transform _objectsParent;
        private ShovelWrapper _shovelWrapper;

        private ObjectPool<Explosion> _pool;

        public static int Count { get; private set; }

        public void Init(Transform objectsParent, ShovelWrapper shovelWrapper)
        {
            _objectsParent = objectsParent;
            _shovelWrapper = shovelWrapper;

            _pool = new ObjectPool<Explosion>(CreateObject, OnGet, OnRelease, OnExplosionDestroy, defaultCapacity: _amount);
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