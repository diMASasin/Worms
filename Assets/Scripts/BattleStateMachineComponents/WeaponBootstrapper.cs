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
        private WeaponSelectorItemFactory _itemFactory = new();
        private WeaponFactory _weaponFactory;
        private IWormEvents _wormEvents;
        private Transform _weaponsParent;
        private ProjectilesBootsrapper _projectilesBootsrapper;

        public WeaponChanger WeaponChanger { get; private set; }
        
        public WeaponBootstrapper(DiContainer container, IEnumerable<WeaponConfig> weaponConfigs, 
            WeaponSelectorItem weaponSelectorItemPrefab, IWormEvents wormEvents, WeaponSelector weaponSelector,
                ProjectilesBootsrapper projectilesBootsrapper)
        {
            _projectilesBootsrapper = projectilesBootsrapper;
            _container = container;
            _wormEvents = wormEvents;
            _weaponSelector = weaponSelector;
            _weaponConfigs = weaponConfigs;
            _weaponSelectorItemPrefab = weaponSelectorItemPrefab;
            
            _weaponsParent = new GameObject("Weapons").transform;
            CreateWeapon();
        }

        public void CreateWeapon()
        {
            _weaponFactory = new WeaponFactory(_projectilesBootsrapper.ProjectilePools, _weaponsParent, _weaponConfigs);
            
            IEnumerable<Weapon> weaponList = _weaponFactory.Create();
            _itemFactory.Create(weaponList, _weaponSelectorItemPrefab, _weaponSelector.ItemParent);
            
            BindServices();
        }

        private void BindServices()
        {
            _container.BindInterfacesAndSelfTo<WeaponSelector>().FromInstance(_weaponSelector).AsSingle();
            _container.Bind<WeaponChanger>().AsSingle().WithArguments(_weaponsParent);
            _container.BindInterfacesAndSelfTo<WeaponFactory>().FromInstance(_weaponFactory).AsSingle();
            _container.BindInterfacesAndSelfTo<WeaponSelectorItemFactory>().FromInstance(_itemFactory).AsSingle();
        }
    }
}