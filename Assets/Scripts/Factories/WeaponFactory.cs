using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Projectiles;
using UnityEngine;
using Weapons;

namespace Factories
{
    public class WeaponFactory : IWeaponShotEvent, IDisposable
    {
        private readonly List<Weapon> _weaponList = new();

        public event Action<float> WeaponShot;

        public void Dispose()
        {
            foreach (var weapon in _weaponList)
                weapon.Shot -= OnShot;
        }

        public List<Weapon> Create(IEnumerable<WeaponConfig> weaponConfigs, Transform projectileSpawnPoint)
        {
            foreach (var config in weaponConfigs)
            {
                var weapon = new Weapon(config);
                
                _weaponList.Add(weapon);

                weapon.Shot += OnShot;
            }

            return _weaponList;
        }

        private void OnShot(float shotPower)
        {
            WeaponShot?.Invoke(shotPower);
        }
    }
}