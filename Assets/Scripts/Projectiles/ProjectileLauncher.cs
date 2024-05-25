using System;
using System.Collections.Generic;
using EventProviders;
using Pools;
using UnityEngine;
using Weapons;

namespace Projectiles
{
    public class ProjectileLauncher : IDisposable
    {
        private readonly IWeaponSelectedEvent _weaponSelectedEvent;
        private readonly IWeaponShotEvent _weaponShotEvent;
        private readonly ISpawnPoint _weaponView;
        private ProjectilePool _pool;

        public ProjectileLauncher(IWeaponSelectedEvent weaponSelectedEvent, IWeaponShotEvent weaponShotEvent, 
            ISpawnPoint weaponView)
        {
            _weaponSelectedEvent = weaponSelectedEvent;
            _weaponShotEvent = weaponShotEvent;
            _weaponView = weaponView;

            _weaponSelectedEvent.WeaponSelected += OnWeaponSelected;
            _weaponShotEvent.WeaponShot += OnWeaponShot;
        }

        public void Dispose()
        {
            _weaponSelectedEvent.WeaponSelected -= OnWeaponSelected;
            _weaponShotEvent.WeaponShot -= OnWeaponShot;
        }

        private void OnWeaponSelected(Weapon weapon, ProjectilePool pool)
        {
            _pool = pool;
        }

        private void OnWeaponShot(float shotPower)
        {
            Projectile projectile = _pool.Get();
            Transform spawnPoint = _weaponView.SpawnPoint;
            Vector3 velocity = _weaponView.SpawnPoint.right * shotPower;

            var transform = projectile.transform;
            transform.position = spawnPoint.position;
            
            projectile.Launch(velocity);
        }
    }
}