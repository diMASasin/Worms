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
using Wind_;
using WormComponents;
using Zenject;

namespace Infrastructure.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private BattleStateMachineData _data;

        private WeaponBootstrapper _weaponBootstrapper;
        private ProjectilesBootsrapper _projectilesBootsrapper;

        public override void InstallBindings()
        {
            Container.BindInstance(_data).AsSingle();

            GameConfig gameConfig = _data.GameConfig;
            Container.BindInstance(gameConfig).AsSingle();
            Container.BindInstance(gameConfig.TimersConfig).AsSingle();
            Container.BindInstance(gameConfig.WormsSpawnerConfig).AsSingle();
            
            Container.BindInterfacesAndSelfTo<Timer>().FromNew().AsTransient();
            Container.BindInstance(_data.Terrain).AsSingle();
            
            Container.BindInterfacesAndSelfTo<FollowingCamera>().FromInstance(_data.FollowingCamera).AsSingle();
            
            Container.Bind<Arrow>().FromComponentInNewPrefab(gameConfig.ArrowPrefab).AsSingle();
            Container.BindInstance(new ShovelWrapper(gameConfig.ShovelPrefab)).AsSingle();

            Container.Bind<ProjectilesBootsrapper>().FromNew().AsSingle().NonLazy();
            Container.Bind<FollowingTimerViewPool>().FromNew().AsSingle();
            Container.Bind<WeaponBootstrapper>().FromNew().AsSingle().WithArguments(_data.WeaponSelector).NonLazy();

            Worm wormPrefab = _data.GameConfig.WormPrefab;
            Container.BindInterfacesAndSelfTo<WormFactory>().FromNew().AsSingle().WithArguments(wormPrefab);
            Container.BindInterfacesAndSelfTo<TeamFactory>().FromNew().AsSingle();
            Container.Bind<WormsSpawner>().FromNew().AsSingle();
            
            Container.BindInterfacesAndSelfTo<WormInfoFactory>().FromNew().AsSingle().WithArguments(gameConfig.WormInfoViewPrefab);
            Container
                .Bind<WindMediator>()
                .FromNew()
                .AsSingle()
                .WithArguments(gameConfig.WindData, _data.WindView);


            Container.Bind<IBattleState>().To<BootstrapBattleState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<BetweenTurnsState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<TurnState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<RetreatState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<ProjectilesWaiting>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<BattleEndState>().FromNew().AsSingle();
            Container.Bind<IBattleState>().To<ExitBattleState>().FromNew().AsSingle();
            
            Container.BindInterfacesAndSelfTo<BattleStateMachine>().FromNew().AsSingle();

            Container.Bind<EndScreen>().FromInstance(_data.EndScreen).AsSingle();
        }
    }
}