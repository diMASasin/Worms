using Configs;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileRotator : ILaunchBehaviour
    {
        private readonly ProjectileConfig _config;
        private readonly Rigidbody2D _rigidbody2D;

        public ProjectileRotator(ProjectileConfig config, Rigidbody2D rigidbody2D)
        {
            _config = config;
            _rigidbody2D = rigidbody2D;
        }

        public void OnLaunch(Vector2 velocity)
        {
            if (_config.LookInVelocityDirection == false)
            {
                var torque = Random.Range(_config.TorqueRange.StartValue, _config.TorqueRange.EndValue);
                _rigidbody2D.AddTorque(Random.Range(-torque, torque));
            }
        }
    }
}