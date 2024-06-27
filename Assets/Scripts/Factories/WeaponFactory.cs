using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Pools;
using Projectiles;
using UnityEngine;
using Weapons;
using Zenject;
using static UnityEngine.Object;

namespace Factories
{
    public class WeaponFactory : IFactory<List<Weapon>>, IWeaponShotEvent, IDisposable
    {
        private readonly List<ProjectilePool> _projectilePools;
        private readonly Transform _weaponsParent;
        private readonly IEnumerable<WeaponConfig> _weaponConfigs;

        private readonly List<Weapon> _weaponList = new();

        public event Action<float, Weapon> WeaponShot;

        public WeaponFactory(List<ProjectilePool> projectilePools, Transform weaponsParent, IEnumerable<WeaponConfig> weaponConfigs)
        {
            _projectilePools = projectilePools;
            _weaponsParent = weaponsParent;
            _weaponConfigs = weaponConfigs;
        }
        
        public void Dispose()
        {
            

            foreach (var weapon in _weaponList)
                weapon.Shot -= OnShot;
        }

        public List<Weapon> Create()
        {
            foreach (var config in _weaponConfigs)
            {
                Weapon weapon = Instantiate(config.WeaponPrefab, _weaponsParent);
                weapon.Init(config);
                weapon.GameObject.SetActive(false);
                
                if(weapon.TryGetComponent(out ProjectileLauncher projectileLauncher) == true)
                    projectileLauncher.Init(_projectilePools);
                
                _weaponList.Add(weapon);

                weapon.Shot += OnShot;
            }

            return _weaponList;
        }

        private void OnShot(float shotPower, Weapon weapon) => 
            WeaponShot?.Invoke(shotPower, weapon);
    }
}