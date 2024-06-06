using System;
using System.Collections.Generic;
using Battle_;
using BattleStateMachineComponents.StatesData;
using Configs;
using Factories;
using InputService;
using Pools;
using Services;
using Timers;
using Weapons;
using WormComponents;
using static UnityEngine.Object;

namespace BattleStateMachineComponents.States
{
    public class BootstrapBattleState : IBattleState, IDisposable
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
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

            _weaponBootstrapper = new WeaponBootstrapper(GameConfig.WeaponConfigs, TurnStateData.WeaponSelector, 
                GameConfig.ItemPrefab);
            
            _poolBootsrapper = new PoolBootsrapper(GameConfig.WeaponConfigs, GameConfig.ExplosionConfig, 
                GameConfig.FollowingTimerViewPrefab);
            
            _wormsBootstraper = new WormsBootstraper(StartStateData.Terrain, services.Single<BattleSettings>(), 
                GameConfig, StartStateData.UIChanger.transform, _data.GlobalBattleData.AliveTeams);
        }

        public void Enter()
        {
            GlobalData.PlayerInput = new PlayerInput(GlobalData.MainInput, GlobalData.FollowingCamera,
                TurnStateData.WeaponSelector);
            
            StartStateData.Terrain.Init(SpawnerConfig.ContactFilter, SpawnerConfig.MaxSlope);
            StartStateData.Terrain.GetEdgesForSpawn();
            
            Arrow arrow = Instantiate(GameConfig.ArrowPrefab);
            ShovelWrapper shovelWrapper = new (Instantiate(GameConfig.ShovelPrefab));

            _poolBootsrapper.InitializePools(shovelWrapper, out var allProjectileEvents, out List<ProjectilePool> projectilePools);
            _weaponBootstrapper.CreateWeapon(projectilePools, out WeaponChanger weaponChanger);
            _wormsBootstraper.SpawnWorms(out WormFactory wormFactory);

            InitializeTimers();

            _data.BetweenTurnsData.Init(GameConfig.WindData, StartStateData.WindView, allProjectileEvents);
            TurnStateData.Init(arrow, allProjectileEvents, weaponChanger, wormFactory);

            _stateSwitcher.SwitchState<BetweenTurnsState>();
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