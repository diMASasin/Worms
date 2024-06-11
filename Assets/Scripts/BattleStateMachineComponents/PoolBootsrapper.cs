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
        private List<ProjectileFactory> _allProjectilesFactories;
        private List<ProjectilePool> _fragmentsPools;
        private List<ProjectileFactory> _fragmentFactories;
        private FollowingTimerViewPool _timerViewPool;
        private ExplosionPool _explosionPool;
        private FragmentsLauncher _fragmentsLauncher;
        private readonly FollowingTimerView _followingTimerViewPrefab;
        private readonly ExplosionConfig _explosionConfig;

        public List<ProjectilePool> ProjectilePools { get; private set; }
        public AllProjectilesEvents ProjectilesEvents { get; private set; }

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
            disposables.AddRange(ProjectilePools);
            disposables.AddRange(_fragmentsPools);
            disposables.AddRange(_fragmentFactories);

            disposables.Add(ProjectilesEvents);
            disposables.Add(_fragmentsLauncher);
            disposables.Add(_explosionPool);
            disposables.Add(_timerViewPool);

            foreach (var disposable in disposables)
                disposable.Dispose();
        }

        public void InitializePools(ShovelWrapper shovel)
        {
            Transform projectilesParent = new GameObject().transform;
            projectilesParent.name = "Projectiles";

            ProjectilePools = new List<ProjectilePool>();
            _fragmentsPools = new List<ProjectilePool>();
            _allProjectilesFactories = new List<ProjectileFactory>();
            _fragmentFactories = new List<ProjectileFactory>();

            foreach (var weaponConfig in _weaponConfigs)
            {
                if (weaponConfig.ProjectileConfig == null)
                    continue;

                var factory = new ProjectileFactory(weaponConfig.ProjectileConfig, projectilesParent);
                var pool = new ProjectilePool(factory, 1);

                _allProjectilesFactories.Add(factory);
                ProjectilePools.Add(pool);
                
                if (weaponConfig.ProjectileConfig.FragmentsConfig == null)
                    continue;
                
                var fragmentsConfig = weaponConfig.ProjectileConfig.FragmentsConfig;
                var fragmentsFactory = new ProjectileFactory(fragmentsConfig, projectilesParent);
                var fragmentPool = new ProjectilePool(fragmentsFactory, 5);

                _fragmentFactories.Add(fragmentsFactory);
                _fragmentsPools.Add(fragmentPool);
                _allProjectilesFactories.Add(fragmentsFactory);
            }

            ProjectilesEvents = new AllProjectilesEvents(_allProjectilesFactories);
            _fragmentsLauncher = new FragmentsLauncher(ProjectilesEvents, _fragmentsPools);
            _timerViewPool = new FollowingTimerViewPool(_followingTimerViewPrefab, ProjectilesEvents);
            _explosionPool = new ExplosionPool(_explosionConfig, shovel, ProjectilesEvents);
        }
    }
}