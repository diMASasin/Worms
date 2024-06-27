using UnityEngine;

namespace Wind_
{
    public class WindEffect
    {
        private readonly Wind _wind;
        private readonly ParticleSystem _particleSystem;

        public WindEffect (Wind wind, ParticleSystem particles)
        {
            _particleSystem = particles;
            _wind = wind;

            _wind.VelocityChanged += OnVelocityChanged;
        }

        private void OnDestroy()
        {
            if (_wind != null) _wind.VelocityChanged -= OnVelocityChanged;
        }

        private void OnVelocityChanged(float velocity)
        {
            var velocityOverLifetime = _particleSystem.velocityOverLifetime;
            velocityOverLifetime.x = -velocity;
        }
    }
}
