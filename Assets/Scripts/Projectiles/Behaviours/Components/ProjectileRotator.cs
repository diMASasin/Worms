using Configs;
using UnityEngine;

namespace Projectiles.Behaviours.LaunchBehaviour
{
    public class ProjectileRotator : MonoBehaviour
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private float _torqueForce;

        private void OnEnable()
        {
            _projectile.Launched += OnLaunched;
        }

        private void OnDisable()
        {
            _projectile.Launched -= OnLaunched;
        }

        private void OnLaunched(Projectile arg1, Vector2 arg2)
        {
            var torque = Random.Range(-_torqueForce, _torqueForce);
            _rigidbody2D.AddTorque(torque);
        }
    }
}