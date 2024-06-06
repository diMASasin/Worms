using System;
using System.Collections.Generic;
using Configs;
using Factories;
using Pools;
using UI;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;

namespace BattleStateMachineComponents.States
{
    public class WeaponBootstrapper : IDisposable
    {
        private readonly IEnumerable<WeaponConfig> _weaponConfigs;
        private readonly WeaponSelector _weaponSelector;
        private readonly WeaponSelectorItem _weaponSelectorItemPrefab;
        private WeaponSelectorItemFactory _itemFactory;
        private WeaponChanger _weaponChanger;
        private WeaponFactory _weaponFactory;

        public WeaponBootstrapper(IEnumerable<WeaponConfig> weaponConfigs, WeaponSelector weaponSelector, 
            WeaponSelectorItem weaponSelectorItemPrefab)
        {
            _weaponConfigs = weaponConfigs;
            _weaponSelector = weaponSelector;
            _weaponSelectorItemPrefab = weaponSelectorItemPrefab;
        }

        public void Dispose()
        {
            _itemFactory.Dispose();
            _weaponChanger.Dispose();
            _weaponFactory.Dispose();
        }

        public void CreateWeapon(List<ProjectilePool> projectilePools, out WeaponChanger weaponChanger)
        {
            Transform weaponsParent = Object.Instantiate(new GameObject("Weapons")).transform;
            
            _itemFactory = new WeaponSelectorItemFactory();
            _weaponFactory = new WeaponFactory(projectilePools, weaponsParent);
            
            IEnumerable<Weapon> weaponList = _weaponFactory.Create(_weaponConfigs);

            _itemFactory.Create(weaponList, _weaponSelectorItemPrefab, _weaponSelector.ItemParent);
            _weaponSelector.Init(_itemFactory);

            _weaponChanger = weaponChanger = new WeaponChanger(_itemFactory, _weaponFactory, weaponsParent);
        }
    }
}