using BattleStateMachineComponents;
using BattleStateMachineComponents.States;
using CameraFollow;
using Configs;
using Factories;
using Pools;
using Spawn;
using Timers;
using UI;
using UnityEngine;
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

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BattleStateMachineData>().FromInstance(_data).AsSingle();

            BindConfigs();

            Container.BindInterfacesAndSelfTo<Timer>().FromNew().AsTransient();
            Container.BindInstance(_data.Terrain).AsSingle();

            Container.BindInterfacesAndSelfTo<FollowingCamera>().FromInstance(_data.FollowingCamera).AsSingle();
            Container.BindInterfacesAndSelfTo<WhenMoveCameraFollower>().FromNew().AsSingle();
            
            Container.Bind<Arrow>().FromComponentInNewPrefab(_battleConfig.ArrowPrefab).AsSingle();
            var shovelWrapper = new ShovelWrapper(_battleConfig.ShovelPrefab);
            Container.BindInstance(shovelWrapper).AsSingle();

            _projectileInstaller = new ProjectileInstaller(Container, _battleConfig, shovelWrapper);
            Container.Bind<ProjectileInstaller>().FromInstance(_projectileInstaller).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FollowingTimerView>()
                .FromComponentInNewPrefab(_battleConfig.FollowingTimerViewPrefab).AsSingle();
            
            BindWeapons();

            BindWorms();

            BindStateMachine();

            Container.Bind<EndScreen>().FromInstance(_data.EndScreen).AsSingle();
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
            Container
                .Bind<WindMediator>()
                .FromNew()
                .AsSingle()
                .WithArguments(_battleConfig.WindData, _data.WindView);

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
    }
}