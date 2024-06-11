using System;
using System.Collections.Generic;
using System.Linq;
using Pools;
using UnityEngine;
using Weapons;

namespace Projectiles
{
    public class ProjectileLauncher : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Weapon _weapon;
        
        private ProjectilePool _pool;

        public void Init(IEnumerable<ProjectilePool> projectilePools)
        {
            _pool = projectilePools.FirstOrDefault(pool => pool.Config == _weapon.Config.ProjectileConfig);

            if (_pool == null)
                throw new NullReferenceException("Projectile pool for this weapon config not found");

            _weapon.Shot += OnWeaponShot;
        }

        public void OnDestroy()
        {
            _weapon.Shot -= OnWeaponShot;
        }

        private void OnWeaponShot(float shotPower, Weapon weapon)
        {
            Projectile projectile = _pool.Get();
            Vector3 velocity = _spawnPoint.right * shotPower;

            var transform = projectile.transform;
            transform.position = _spawnPoint.position;
            
            projectile.Launch(velocity);
        }
    }
}