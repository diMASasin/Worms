using System.Collections.Generic;
using System.Linq;
using Configs;
using Explosion_;
using Factories;
using Pools;
using Projectiles;
using Projectiles.Behaviours;
using UnityEngine;
using Zenject;

namespace BattleStateMachineComponents.States
{
    public class ProjectileInstaller
    {
        private WeaponConfig[] _weaponConfigs;
        private FollowingTimerViewPool _timerViewPool;
        private ExplosionPool _explosionPool;
        private FragmentsLauncher _fragmentsLauncher;
        private ExplosionPool _fragmentsExplosionPool;
        private DiContainer _container;
        private readonly BattleStateMachineData _data;

        public List<ProjectilePool> ProjectilePools { get; private set; }
        public AllProjectilesEvents ProjectileEvents { get; private set; }
        private readonly List<ProjectileFactory> _projectileAndFragmentFactories = new();
        private List<ProjectilePool> _fragmentPools;
        private IShovel _shovel;

        public ProjectileInstaller(DiContainer container, BattleConfig battleConfig, IShovel shovel)
        {
            _shovel = shovel;
            _container = container;
            _weaponConfigs = battleConfig.WeaponConfigs;
            
            InitializePools();
        }
        
        public void InitializePools()
        {
            Transform projectilesParent = new GameObject("Projectiles").transform;
            
            var projectileConfigs = _weaponConfigs
                    .Select(config => config.ProjectileConfig)
                    .Where(projectileConfig => projectileConfig != null)
                    .Distinct();

            BindProjectilePools(projectileConfigs, projectilesParent, out List<ProjectilePool> projectilePools);
            ProjectilePools = projectilePools;

            var fragmentConfigs = projectileConfigs
                    .Select(config => config.FragmentsConfig)
                    .Where(fragmentsConfig => fragmentsConfig != null)
                    .Distinct();
            
            BindProjectilePools(fragmentConfigs, projectilesParent, out List<ProjectilePool> fragmentPools);
            _fragmentPools = fragmentPools;

            ProjectileEvents = new AllProjectilesEvents(_projectileAndFragmentFactories);
            _container.BindInterfacesAndSelfTo<AllProjectilesEvents>().FromInstance(ProjectileEvents).AsSingle();

            var explosionConfigs = projectileConfigs
                    .Concat(fragmentConfigs)
                    .Select(config => config.ExplosionConfig)
                    .Where(explosionConfig => explosionConfig != null)
                    .Distinct();

            var explosionsParent = new GameObject("Explosions").transform;
            List<ExplosionPool> explosionPools = new();
            
            foreach (var explosionConfig in explosionConfigs)
            {
                var explosionPool = new ExplosionPool(_shovel, explosionConfig, explosionsParent);
                explosionPools.Add(explosionPool);
                _container.BindInterfacesAndSelfTo<ExplosionPool>().FromInstance(explosionPool);
            }

            _container.BindInterfacesAndSelfTo<AllExplosionEvents>().FromNew().AsSingle().WithArguments(explosionPools);
            _container.BindInterfacesAndSelfTo<Exploder>().FromNew().AsSingle().WithArguments(explosionPools);
            _container.BindInterfacesAndSelfTo<FragmentsLauncher>().FromNew().AsSingle().WithArguments(_fragmentPools);
            _container.BindInterfacesAndSelfTo<FollowingTimerViewPool>().FromNew().AsSingle();
        }

        private void BindProjectilePools(IEnumerable<ProjectileConfig> projectileConfigs, Transform projectilesParent,
            out List<ProjectilePool> pools)
        {
            pools = new List<ProjectilePool>();
            
            foreach (var projectileConfig in projectileConfigs)
            {
                if (projectileConfig == null)
                    continue;

                var factory = new ProjectileFactory(projectileConfig, projectilesParent);
                var pool = new ProjectilePool(factory, 1);

                _container.BindInterfacesAndSelfTo<ProjectileFactory>().FromInstance(factory);
                _container.BindInterfacesAndSelfTo<ProjectilePool>().FromInstance(pool);

                pools.Add(pool);
                _projectileAndFragmentFactories.Add(factory);
            }
        }
    }
}