using System;
using System.Collections.Generic;
using Pools;

namespace Explosion_
{
    public class AllExplosionEvents : IExplosionEvents, IDisposable
    {
        private readonly List<ExplosionPool> _explosionPools;
        
        public event Action<Explosion> Exploded;
        public event Action<Explosion> AnimationStopped;

        public AllExplosionEvents(List<ExplosionPool> explosionPools)
        {
            _explosionPools = explosionPools;

            foreach (var pool in _explosionPools)
            {
                pool.Exploded += OnExploded;
                pool.AnimationStopped += OnAnimationStopped;
            }
        }

        public void Dispose()
        {
            foreach (var pool in _explosionPools)
            {
                pool.Exploded -= OnExploded;
                pool.AnimationStopped -= OnAnimationStopped;
            }
        }

        private void OnExploded(Explosion explosion) => Exploded?.Invoke(explosion);

        private void OnAnimationStopped(Explosion explosion) => AnimationStopped?.Invoke(explosion);
    }
}