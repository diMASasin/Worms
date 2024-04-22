using Configs;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileRotator : IProjectileLauchModifier
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
                //5, 7
                var torque = Random.Range(_config.RandomizedTorqueForce.StartValue, _config.RandomizedTorqueForce.EndValue);
                _rigidbody2D.AddTorque(Random.Range(-torque, torque));
            }
        }
    }
}