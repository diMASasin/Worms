using System;
using System.Collections.Generic;
using Configs;
using Factories;
using Pools;
using Projectiles;
using Projectiles.Behaviours;
using UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BattleStateMachineComponents.States
{
    public class PoolBootsrapper : IDisposable
    {
        private readonly WeaponConfig[] _weaponConfigs;
        private List<ProjectilePool> _projectilePools;
        private List<ProjectileFactory> _allProjectilesFactories;
        private List<ProjectilePool> _fragmentsPools;
        private List<ProjectileFactory> _fragmentFactories;
        private FollowingTimerViewPool _timerViewPool;
        private ExplosionPool _explosionPool;
        private FragmentsLauncher _fragmentsLauncher;
        private AllProjectilesEvents _allProjectilesEvents;
        private readonly FollowingTimerView _followingTimerViewPrefab;
        private readonly ExplosionConfig _explosionConfig;

        public PoolBootsrapper(WeaponConfig[] weaponConfigs, ExplosionConfig explosionConfig, 
            FollowingTimerView followingTimerViewPrefab)
        {
            _weaponConfigs = weaponConfigs;
            _explosionConfig = explosionConfig;
            _followingTimerViewPrefab = followingTimerViewPrefab;
        }

        public void Dispose()
        {
            List<IDisposable> disposables = new();
            
            disposables.AddRange(_allProjectilesFactories);
            disposables.AddRange(_projectilePools);
            disposables.AddRange(_fragmentsPools);
            disposables.AddRange(_fragmentFactories);
            
            disposables.Add(_allProjectilesEvents);
            disposables.Add(_fragmentsLauncher);
            disposables.Add(_explosionPool);
            disposables.Add(_timerViewPool);
            
            foreach (var disposable in disposables)
                disposable.Dispose();
        }

        public void InitializePools(ShovelWrapper shovel, out AllProjectilesEvents allProjectileEvents, 
            out List<ProjectilePool> projectilePools)
        {
            var projectilesParent = Object.Instantiate(new GameObject()).transform;
            projectilesParent.name = "Projectiles";

            _projectilePools = projectilePools = new List<ProjectilePool>();
            _fragmentsPools = new List<ProjectilePool>();
            _allProjectilesFactories = new List<ProjectileFactory>();
            _fragmentFactories = new List<ProjectileFactory>();
            
            foreach (var weaponConfig in _weaponConfigs)
            {
                var factory = new ProjectileFactory(weaponConfig.ProjectileConfig, projectilesParent);
                var pool = new ProjectilePool(factory, 1);

                _allProjectilesFactories.Add(factory);
                _projectilePools.Add(pool);

                var fragmentsConfig = weaponConfig.ProjectileConfig.FragmentsConfig;
                var fragmentsFactory = new ProjectileFactory(fragmentsConfig, projectilesParent);
                var fragmentPool = new ProjectilePool(fragmentsFactory, 5);
                
                _fragmentFactories.Add(fragmentsFactory);
                _fragmentsPools.Add(fragmentPool);
                _allProjectilesFactories.Add(fragmentsFactory);
            }

            _allProjectilesEvents = allProjectileEvents = new AllProjectilesEvents(_allProjectilesFactories);
            _fragmentsLauncher = new FragmentsLauncher(_allProjectilesEvents, _fragmentsPools);
            _timerViewPool = new FollowingTimerViewPool(_followingTimerViewPrefab, _allProjectilesEvents);
            _explosionPool = new ExplosionPool(_explosionConfig, shovel, _allProjectilesEvents);
        }
    }
}