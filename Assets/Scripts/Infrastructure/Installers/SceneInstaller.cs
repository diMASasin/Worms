using BattleStateMachineComponents;
using BattleStateMachineComponents.States;
using CameraFollow;
using Configs;
using Explosion_;
using Factories;
using ScriptBoy.Digable2DTerrain.Scripts;
using Spawn;
using Timers;
using UI;
using UnityEngine;
using Water;
using Weapons;
using Wind_;
using WormComponents;
using Zenject;

namespace Infrastructure.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private BattleStateMachineData _data;

        private ProjectileInstaller _projectileInstaller;
        private BattleConfig _battleConfig;
        private ShovelWrapper _shovelWrapper;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BattleStateMachineData>().FromInstance(_data).AsSingle();

            BindConfigs();

            Container.BindInstance(_data.Terrain).AsSingle();
            Container.BindInterfacesAndSelfTo<Timer>().FromNew().AsTransient();
            Container.Bind<Arrow>().FromComponentInNewPrefab(_battleConfig.ArrowPrefab).AsSingle();
            BindShovel();
            Container.Bind<Explosion>().FromComponentInNewPrefab(_battleConfig.ExplosionConfig.Prefab).AsSingle();

            BindProjectile();
            BindCamera();
            
            Container.Bind<WindMediator>().FromNew().AsSingle().WithArguments(_battleConfig.WindData, _data.WindView,
                _projectileInstaller.ProjectileEvents);
            BindWater();

            BindWeapons();
            BindWorms();
            BindUI();
            BindStateMachine();
        }

        private void BindShovel()
        {
            var shovel = Instantiate(_battleConfig.ShovelPrefab);
            Container.BindInterfacesAndSelfTo<Shovel>().FromInstance(shovel).AsSingle().NonLazy();
            _shovelWrapper = new ShovelWrapper(shovel);
            Container.BindInterfacesAndSelfTo<ShovelWrapper>().FromInstance(_shovelWrapper).AsSingle().NonLazy();
        }

        private void BindCamera()
        {
            // Container.BindInterfacesAndSelfTo<FollowingCamera>().FromInstance(_data.FollowingCamera).AsSingle();
            Container.BindInterfacesAndSelfTo<CinemachineFollowingCamera>()
                .FromInstance(_data.CinemachineFollowingCamera).AsSingle();
            Container.BindInterfacesAndSelfTo<FollowingCameraEventsListener>().FromNew().AsSingle();
        }

        private void BindWater()
        {
            _data.WaterLevelIncreaser.Init(_battleConfig.WaterStep);
            Container.Bind<StylizedWater.Scripts.StylizedWater>().FromInstance(_data.StylizedWater).AsSingle();
            Container.Bind<WaterLevelIncreaser>().FromInstance(_data.WaterLevelIncreaser).AsSingle();
            Container.Bind<WaterVelocityChanger>().FromNew().AsSingle();
        }

        private void BindProjectile()
        {
            _projectileInstaller = new ProjectileInstaller(Container, _battleConfig, _shovelWrapper);
            Container.Bind<ProjectileInstaller>().FromInstance(_projectileInstaller).AsSingle();
            Container.BindInterfacesAndSelfTo<FollowingTimerView>()
                .FromInstance(_battleConfig.FollowingTimerViewPrefab).AsSingle();
        }

        private void BindConfigs()
        {
            _battleConfig = _data.BattleConfig;
            Container.BindInstance(_battleConfig).AsSingle();
            Container.BindInstance(_battleConfig.TimersConfig).AsSingle();
            Container.BindInstance(_battleConfig.WormsSpawnerConfig).AsSingle();
            Container.BindInstance(_battleConfig.WeaponConfigs).AsSingle();
        }

        private void BindWeapons()
        {
            Transform weaponsParent = new GameObject("Weapons").transform;
            
            Container.BindInterfacesAndSelfTo<WeaponFactory>().FromNew().AsSingle()
                .WithArguments(_projectileInstaller.ProjectilePools, weaponsParent, _battleConfig.WeaponConfigs);
            
            Container.BindInterfacesAndSelfTo<WeaponSelector>().FromInstance(_data.WeaponSelector).AsSingle();
            Container.BindInterfacesAndSelfTo<WeaponChanger>().FromNew().AsSingle().WithArguments(weaponsParent);
            
            Container.BindInterfacesAndSelfTo<WeaponSelectorItemFactory>().FromNew().AsSingle()
                .WithArguments(_battleConfig.ItemPrefab, _data.WeaponSelector.ItemParent);
        }

        private void BindWorms()
        {
            Worm wormPrefab = _data.BattleConfig.WormPrefab;
            Container.BindInterfacesAndSelfTo<WormFactory>().FromNew().AsSingle().WithArguments(wormPrefab);
            Container.BindInterfacesAndSelfTo<TeamFactory>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<WormInfoFactory>().FromNew().AsSingle()
                .WithArguments(_battleConfig.WormInfoViewPrefab);

            Container.Bind<WormsSpawner>().FromNew().AsSingle();
        }

        private void BindStateMachine()
        {
            Container.Bind<IBattleState>().To<BootstrapBattleState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<BetweenTurnsState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<TurnState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<RetreatState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<ProjectilesWaiting>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<BattleEndState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<ExitBattleState>().FromNew().AsSingle();

            Container.BindInterfacesAndSelfTo<BattleStateMachine>().FromNew().AsSingle();
        }

        private void BindUI()
        {
            Container.Bind<EndScreen>().FromInstance(_data.EndScreen).AsSingle();
        }
    }
}