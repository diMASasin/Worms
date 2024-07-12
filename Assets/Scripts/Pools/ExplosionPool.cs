using System;
using Configs;
using Explosion_;
using Projectiles;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Object;

namespace Pools
{
    public class ExplosionPool : IDisposable
    {
        public readonly ExplosionConfig Config;
        private int _amount;

        private readonly Transform _explosionsParent;
        private readonly IShovel _shovelWrapper;

        private readonly ObjectPool<Explosion> _pool;
        private readonly IProjectileEvents _projectileEvents;
        private readonly IShovel _shovel;

        public static int Count { get; private set; }

        public event Action<Explosion> Exploded;
        public event Action<Explosion> AnimationStopped;

        public ExplosionPool(IShovel shovel, ExplosionConfig config, Transform explosionsParent, int capacity = 5)
        {
            Config = config;
            _explosionsParent = explosionsParent;
            _shovel = shovel;
            
            _pool = new ObjectPool<Explosion>(CreateObject, OnGet, OnRelease, OnExplosionDestroy, defaultCapacity: capacity);
        }

        public void Dispose() => _pool?.Dispose();

        public Explosion Get() => _pool.Get();

        private Explosion CreateObject()
        {
            var explosion = Instantiate(Config.Prefab, _explosionsParent);
            explosion.Init(_shovel);
            
            explosion.AnimationStopped += OnAnimationStopped;
            explosion.Exploded += OnExploded;

            explosion.gameObject.SetActive(false);
            return explosion;
        }

        private void OnAnimationStopped(Explosion explosion)
        {
            _pool.Release(explosion);
            AnimationStopped.Invoke(explosion);
        }

        private void OnExploded(Explosion explosion)
        {
            Exploded?.Invoke(explosion);
        }

        private void OnExplosionDestroy(Explosion explosion)
        {
            explosion.AnimationStopped -= OnAnimationStopped;
            explosion.Exploded -= OnExploded;
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