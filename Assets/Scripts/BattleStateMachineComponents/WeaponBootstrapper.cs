using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Factories;
using Pools;
using Services;
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
        private readonly IWeaponInput _weaponInput;
        private readonly IWeaponSelectorInput _weaponSelectorInput;
        private WeaponSelectorItemFactory _itemFactory;
        private WeaponFactory _weaponFactory;

        public WeaponChanger WeaponChanger { get; private set; }
        
        public WeaponBootstrapper(IEnumerable<WeaponConfig> weaponConfigs, WeaponSelector weaponSelector, 
            WeaponSelectorItem weaponSelectorItemPrefab, AllServices services)
        {
            _weaponConfigs = weaponConfigs;
            _weaponSelector = weaponSelector;
            _weaponSelectorItemPrefab = weaponSelectorItemPrefab;
            _weaponInput = services.Single<IWeaponInput>();
            _weaponSelectorInput = services.Single<IWeaponSelectorInput>();
        }

        public void Dispose()
        {
            _itemFactory.Dispose();
            WeaponChanger.Dispose();
            _weaponFactory.Dispose();
        }

        public void CreateWeapon(List<ProjectilePool> projectilePools, IWormEvents wormEvents)
        {
            Transform weaponsParent = Object.Instantiate(new GameObject("Weapons")).transform;
            
            _itemFactory = new WeaponSelectorItemFactory();
            _weaponFactory = new WeaponFactory(projectilePools, weaponsParent, _itemFactory, _weaponInput);
            
            IEnumerable<Weapon> weaponList = _weaponFactory.Create(_weaponConfigs);

            _itemFactory.Create(weaponList, _weaponSelectorItemPrefab, _weaponSelector.ItemParent);
            _weaponSelector.Init(_itemFactory, _weaponSelectorInput);

            WeaponChanger = new WeaponChanger(_itemFactory, _weaponFactory, weaponsParent, wormEvents);
        }
    }
}