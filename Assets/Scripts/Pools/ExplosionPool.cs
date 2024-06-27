using System;
using Configs;
using Projectiles;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;
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
        private readonly DiContainer _container;
        private IShovel _shovel;

        public static int Count { get; private set; }

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