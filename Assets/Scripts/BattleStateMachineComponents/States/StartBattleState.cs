using System;
using Battle_;
using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Configs;
using EventProviders;
using Projectiles;
using Services;
using Timers;
using UltimateCC;
using UnityEngine;
using Weapons;
using WormComponents;
using static UnityEngine.Object;

namespace BattleStateMachineComponents.States
{
    public class BootstrapBattleState : IBattleState, IDisposable
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
        private readonly AllServices _services;
        private readonly WeaponBootstrapper _weaponBootstrapper;
        private readonly PoolBootsrapper _poolBootsrapper;
        private readonly WormsBootstraper _wormsBootstraper;
        
        private GameConfig GameConfig => StartStateData.GameConfig;
        private GlobalBattleData GlobalData => _data.GlobalBattleData;
        private StartStateData StartStateData => _data.StartStateData;
        private TurnStateData TurnStateData => _data.TurnStateData;
        private WormsSpawnerConfig SpawnerConfig => StartStateData.WormsSpawner.Config;

        public BootstrapBattleState(IStateSwitcher stateSwitcher, BattleStateMachineData data, AllServices services)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
            _services = services;
            Transform healthParent = StartStateData.UIChanger.transform;

            _weaponBootstrapper = new WeaponBootstrapper(GameConfig.WeaponConfigs, TurnStateData.WeaponSelector, 
                GameConfig.ItemPrefab, _services);
            
            _poolBootsrapper = new PoolBootsrapper(GameConfig.WeaponConfigs, GameConfig.ExplosionConfig, 
                GameConfig.FollowingTimerViewPrefab);

            _wormsBootstraper = new WormsBootstraper(StartStateData.Terrain, services.Single<IBattleSettings>(), 
                StartStateData.WormsSpawner, GameConfig, healthParent, GlobalData.AliveTeams);
        }

        public void Enter()
        {
            GlobalData.Init(GameConfig.TimersConfig, AllServices.Container.Single<IMovementInput>());
            
            InitializeTerrain();

            CreateObjects(out var shovelWrapper, out var arrow);

            InitializeBootstrappers(shovelWrapper);

            InitializeTimers();

            InitializeStatesData(arrow, _weaponBootstrapper.WeaponChanger);
            
            _stateSwitcher.SwitchState<BetweenTurnsState>();
        }

        private void InitializeTerrain()
        {
            StartStateData.Terrain.Init(SpawnerConfig.ContactFilter, SpawnerConfig.MaxSlope);
            StartStateData.Terrain.GetEdgesForSpawn();
        }

        private void CreateObjects(out ShovelWrapper shovelWrapper, out Arrow arrow)
        {
            arrow = Instantiate(GameConfig.ArrowPrefab);
            shovelWrapper = new(Instantiate(GameConfig.ShovelPrefab));
        }

        private void InitializeBootstrappers(ShovelWrapper shovelWrapper)
        {
            _poolBootsrapper.InitializePools(shovelWrapper);
            _wormsBootstraper.SpawnWorms();
            _weaponBootstrapper.CreateWeapon(_poolBootsrapper.ProjectilePools, _wormsBootstraper.WormEvents, GlobalData);
        }

        private void InitializeStatesData(Arrow arrow, WeaponChanger weaponChanger)
        {
            IProjectileEvents projectileEvents = _poolBootsrapper.ProjectilesEvents;
            IWeaponShotEvent weaponShotEvent = _weaponBootstrapper.WeaponShotEvent;
            
            _data.BetweenTurnsData.Init(GameConfig.WindData, StartStateData.WindView, projectileEvents);
            TurnStateData.Init(arrow, projectileEvents, weaponChanger, _wormsBootstraper.WormEvents, weaponShotEvent);
            GlobalData.FollowingCamera.Init(_services.Single<ICameraInput>());
        }

        private void InitializeTimers()
        {
            Timer globalTimer = GlobalData.GlobalTimer;
            GlobalBattleData globalBattleData = _data.GlobalBattleData;
            float globalTime = globalBattleData.TimersConfig.GlobalTime;

            globalTimer.Start(globalTime, () => GlobalData.Water.AllowIncreaseWaterLevel());
            globalTimer.Pause();

            StartStateData.GlobalTimerView.Init(globalTimer, TimerFormattingStyle.MinutesAndSeconds);
            StartStateData.TurnTimerView.Init(globalBattleData.TurnTimer, TimerFormattingStyle.Seconds);
        }

        public void Exit() { }

        public void Tick(){}

        public void FixedTick(){ }

        public void HandleInput(){}
        
        public void Dispose()
        {
            _wormsBootstraper.Dispose();
            _weaponBootstrapper.Dispose();
            _poolBootsrapper.Dispose();
        }
    }
}