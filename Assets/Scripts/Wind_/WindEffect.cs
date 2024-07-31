using System;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Wind_
{
    public class WindEffect : IDisposable
    {
        private readonly Wind _wind;
        private readonly ParticleSystem _particleSystem;

        public WindEffect (Wind wind, ParticleSystem particles)
        {
            _particleSystem = particles;
            _wind = wind;

            _wind.VelocityChanged += OnVelocityChanged;
        }

        public void Dispose()
        {
            if (_wind != null) _wind.VelocityChanged -= OnVelocityChanged;
        }

        private void OnVelocityChanged(float velocity)
        {
            VelocityOverLifetimeModule velocityOverLifetime = _particleSystem.velocityOverLifetime;
            velocityOverLifetime.xMultiplier = velocity;
        }
    }
}
