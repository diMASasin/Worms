using System;
using System.Collections.Generic;
using BattleStateMachineComponents.StatesData;
using Configs;
using EventProviders;
using Factories;
using Pools;
using UI;
using UnityEngine;
using Weapons;
using WormComponents;
using Zenject;
using Object = UnityEngine.Object;

namespace BattleStateMachineComponents.States
{
    public class WeaponBootstrapper
    {
        private readonly BattleStateMachineData _data;
        private IEnumerable<WeaponConfig> _weaponConfigs;
        private readonly WeaponSelector _weaponSelector;
        private WeaponSelectorItem _weaponSelectorItemPrefab;
        private DiContainer _container;
        private WeaponSelectorItemFactory _itemFactory;
        private WeaponFactory _weaponFactory;
        private IWormEvents _wormEvents;
        private Transform _weaponsParent;
        private ProjectileInstaller _projectileInstaller;
        private IEnumerable<Weapon> _weaponList;

        public WeaponChanger WeaponChanger { get; private set; }
        
        public WeaponBootstrapper(DiContainer container, IEnumerable<WeaponConfig> weaponConfigs, 
            WeaponSelectorItem weaponSelectorItemPrefab, IWormEvents wormEvents, WeaponSelector weaponSelector,
                ProjectileInstaller projectileInstaller)
        {
            _projectileInstaller = projectileInstaller;
            _container = container;
            _wormEvents = wormEvents;
            _weaponSelector = weaponSelector;
            _weaponConfigs = weaponConfigs;
            _weaponSelectorItemPrefab = weaponSelectorItemPrefab;
            
            BindServices();
        }

        private void BindServices()
        {
            
        }
    }
}