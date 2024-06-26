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

        private ProjectilesBootsrapper _projectilesBootsrapper;
        private GameConfig _gameConfig;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BattleStateMachineData>().FromInstance(_data).AsSingle();

            _gameConfig = _data.GameConfig;
            Container.BindInstance(_gameConfig).AsSingle();
            Container.BindInstance(_gameConfig.TimersConfig).AsSingle();
            Container.BindInstance(_gameConfig.WormsSpawnerConfig).AsSingle();
            Container.BindInstance(_gameConfig.WeaponConfigs).AsSingle();
            
            Container.BindInterfacesAndSelfTo<Timer>().FromNew().AsTransient();
            Container.BindInstance(_data.Terrain).AsSingle();

            Container.BindInterfacesAndSelfTo<FollowingCamera>().FromInstance(_data.FollowingCamera).AsSingle();
            Container.BindInterfacesAndSelfTo<WhenMoveCameraFollower>().FromNew().AsSingle();
            
            Container.Bind<Arrow>().FromComponentInNewPrefab(_gameConfig.ArrowPrefab).AsSingle();
            var shovelWrapper = new ShovelWrapper(_gameConfig.ShovelPrefab);
            Container.BindInstance(shovelWrapper).AsSingle();

            _projectilesBootsrapper = new ProjectilesBootsrapper(Container, _gameConfig, shovelWrapper);
            Container.Bind<ProjectilesBootsrapper>().FromInstance(_projectilesBootsrapper).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FollowingTimerView>()
                .FromComponentInNewPrefab(_gameConfig.FollowingTimerViewPrefab).AsSingle();
            
            BindWeapons();

            Worm wormPrefab = _data.GameConfig.WormPrefab;
            Container.BindInterfacesAndSelfTo<WormFactory>().FromNew().AsSingle().WithArguments(wormPrefab);
            Container.BindInterfacesAndSelfTo<TeamFactory>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<WormInfoFactory>().FromNew().AsSingle().WithArguments(_gameConfig.WormInfoViewPrefab);
            Container
                .Bind<WindMediator>()
                .FromNew()
                .AsSingle()
                .WithArguments(_gameConfig.WindData, _data.WindView);

            Container.Bind<WormsSpawner>().FromNew().AsSingle();

            BindStateMachine();

            Container.Bind<EndScreen>().FromInstance(_data.EndScreen).AsSingle();
        }

        private void BindWeapons()
        {
            Transform weaponsParent = new GameObject("Weapons").transform;
            
            Container.BindInterfacesAndSelfTo<WeaponFactory>().FromNew().AsSingle()
                .WithArguments(_projectilesBootsrapper.ProjectilePools, weaponsParent, _gameConfig.WeaponConfigs);
            
            Container.BindInterfacesAndSelfTo<WeaponSelector>().FromInstance(_data.WeaponSelector).AsSingle();
            Container.BindInterfacesAndSelfTo<WeaponChanger>().FromNew().AsSingle().WithArguments(weaponsParent);
            
            Container.BindInterfacesAndSelfTo<WeaponSelectorItemFactory>().FromNew().AsSingle()
                .WithArguments(_gameConfig.ItemPrefab, _data.WeaponSelector.ItemParent);
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