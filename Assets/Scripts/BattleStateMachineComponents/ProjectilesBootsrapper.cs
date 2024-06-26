using System.Collections.Generic;
using System.Linq;
using Configs;
using Factories;
using Pools;
using Projectiles;
using Projectiles.Behaviours;
using UI;
using UnityEngine;
using Zenject;

namespace BattleStateMachineComponents.States
{
    public class ProjectilesBootsrapper
    {
        private WeaponConfig[] _weaponConfigs;
        private FollowingTimerViewPool _timerViewPool;
        private ExplosionPool _explosionPool;
        private FragmentsLauncher _fragmentsLauncher;
        private FollowingTimerView _followingTimerViewPrefab;
        private ExplosionConfig _explosionConfig;
        private ExplosionPool _fragmentsExplosionPool;
        private ShovelWrapper _shovel;
        private DiContainer _container;
        private readonly BattleStateMachineData _data;

        public List<ProjectilePool> ProjectilePools { get; private set; }
        private AllProjectilesEvents _projectilesEvents;
        
        public ProjectilesBootsrapper(DiContainer container, GameConfig gameConfig, ShovelWrapper shovel)
        {
            _container = container;
            _shovel = shovel;
            _weaponConfigs = gameConfig.WeaponConfigs;
            _explosionConfig = gameConfig.ExplosionConfig;
            _followingTimerViewPrefab = gameConfig.FollowingTimerViewPrefab;
            
            InitializePools();
        } 

        public void InitializePools()
        {
            Transform projectilesParent = new GameObject("Projectiles").transform;

            var projectileAndFragmentFactories = new List<ProjectileFactory>();
            ProjectilePools = new List<ProjectilePool>();
            var fragmentsPools = new List<ProjectilePool>();

            IEnumerable<ProjectileConfig> projectileConfigs = 
                _weaponConfigs
                    .Select(config => config.ProjectileConfig)
                    .Where(projectileConfig => projectileConfig != null)
                    .Distinct();
            
            foreach (var projectileConfig in projectileConfigs)
            {
                if (projectileConfig == null)
                    continue;

                var factory = new ProjectileFactory(projectileConfig, projectilesParent);
                var pool = new ProjectilePool(factory, 1);

                ProjectilePools.Add(pool);
                projectileAndFragmentFactories.Add(factory);
            }
            
            IEnumerable<ProjectileConfig> fragmentConfigs = 
                projectileConfigs
                    .Select(config => config.FragmentsConfig)
                    .Where(fragmentsConfig => fragmentsConfig != null)
                    .Distinct();

            foreach (var fragmentsConfig in fragmentConfigs)
            {
                if(fragmentsConfig == null)
                    continue;
                
                var fragmentsFactory = new ProjectileFactory(fragmentsConfig, projectilesParent);
                var fragmentPool = new ProjectilePool(fragmentsFactory, 5);

                fragmentsPools.Add(fragmentPool);
                projectileAndFragmentFactories.Add(fragmentsFactory);
            }

            AllProjectilesEvents allProjectileEvents = new(projectileAndFragmentFactories);
            _container.BindInterfacesAndSelfTo<AllProjectilesEvents>().FromInstance(allProjectileEvents).AsSingle();
            
            IEnumerable<ExplosionConfig> explosionConfigs = 
                projectileConfigs
                    .Concat(fragmentConfigs)
                    .Select(config => config.ExplosionConfig)
                    .Where(explosionConfig => explosionConfig != null)
                    .Distinct();

            List<ExplosionPool> explosionPools = new();
            
            foreach (var explosionConfig in explosionConfigs)
                explosionPools.Add(new ExplosionPool(explosionConfig, _shovel, allProjectileEvents));

            _container.BindInterfacesAndSelfTo<List<ExplosionPool>>().FromInstance(explosionPools).AsSingle();

            _container.BindInterfacesAndSelfTo<FragmentsLauncher>().FromNew().AsSingle().WithArguments(fragmentsPools);
            _container.BindInterfacesAndSelfTo<FollowingTimerViewPool>().FromNew().AsSingle();
        }
    }
}