using System;
using System.Collections.Generic;
using Battle_;
using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Configs;
using EventProviders;
using Factories;
using InputService;
using Pools;
using Projectiles;
using Projectiles.Behaviours;
using Timers;
using UnityEngine;
using Weapons;
using WormComponents;
using static UnityEngine.Object;

namespace BattleStateMachineComponents.States
{
    public class StartBattleState : IBattleState
    {
        private readonly List<IDisposable> _toDispose = new ();

        private Transform TeamHealthParent => StartStateData.UIChanger.transform;
        private TimersConfig TimersConfig => _data.GlobalBattleData.TimersConfig;
        private GameConfig GameConfig => StartStateData.GameConfig;
        private float WaterStep => GameConfig.WaterStep;
        private ITeamDiedEventProvider TeamDiedEvent { get; set; }
        private GlobalBattleData GlobalData => _data.GlobalBattleData;
        private MainInput MainInput => GlobalData.MainInput;
        private FollowingCamera FollowingCamera => GlobalData.FollowingCamera;

        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
        private WormInfoFactory _wormInfoFactory;
        private StartStateData StartStateData => _data.StartStateData;
        private TurnStateData TurnStateData => _data.TurnStateData;
        private BetweenTurnsStateData BetweenTurnsData => _data.BetweenTurnsData;
        private WormsSpawnerConfig SpawnerConfig => StartStateData.WormsSpawner.Config;
        private Timer GlobalTimer => GlobalData.GlobalTimer;

        public StartBattleState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            GlobalData.PlayerInput = new PlayerInput(MainInput, FollowingCamera, TurnStateData.WeaponSelector);
            StartStateData.Terrain.Init(SpawnerConfig.ContactFilter, SpawnerConfig.MaxSlope);
            StartStateData.Terrain.GetEdgesForSpawn();
            
            Arrow arrow = Instantiate(GameConfig.ArrowPrefab);
            ShovelWrapper shovelWrapper = new (Instantiate(GameConfig.ShovelPrefab));

            InitializePools(shovelWrapper, out var allProjectileEvents, out var projectilePools);
            CreateWeapon(projectilePools, out WeaponChanger weaponChanger);

            BetweenTurnsData.Init(GameConfig.WindData, StartStateData.WindView, allProjectileEvents, WaterStep);
            
            SpawnWorms();

            GlobalTimer.Start(TimersConfig.GlobalTime, () => GlobalData.Water.AllowIncreaseWaterLevel());
            GlobalTimer.Pause();
            
            StartStateData.GlobalTimerView.Init(GlobalData.GlobalTimer, TimerFormattingStyle.MinutesAndSeconds);
            StartStateData.TurnTimerView.Init(_data.GlobalBattleData.TurnTimer, TimerFormattingStyle.Seconds);
            
            TurnStateData.Init(arrow, allProjectileEvents, weaponChanger);
            
            TeamDiedEvent.TeamDied += OnTeamDied;
            _stateSwitcher.SwitchState<BetweenTurnsState>();
        }

        private void InitializePools(ShovelWrapper shovel, out AllProjectilesEvents allProjectileEvents, 
            out List<ProjectilePool> projectilePools)
        {
            var projectilesParent = Instantiate(new GameObject()).transform;
            projectilesParent.name = "Projectile Pools";
            
            projectilePools = new List<ProjectilePool>();
            var fragmentsPools = new List<ProjectilePool>();
            var allProjectilesFactories = new List<ProjectileFactory>();

            foreach (var weaponConfig in GameConfig.WeaponConfigs)
            {
                var factory = new ProjectileFactory(weaponConfig.ProjectileConfig, projectilesParent);
                var pool = new ProjectilePool(factory, 1);

                allProjectilesFactories.Add(factory);
                projectilePools.Add(pool);

                var fragmentsConfig = weaponConfig.ProjectileConfig.FragmentsConfig;
                var fragmentsFactory = new ProjectileFactory(fragmentsConfig, projectilesParent);
                var fragmentPool = new ProjectilePool(fragmentsFactory, 5);
                fragmentsPools.Add(fragmentPool);
                allProjectilesFactories.Add(fragmentsFactory);
                
                _toDispose.Add(factory);
                _toDispose.Add(pool);
                _toDispose.Add(fragmentsFactory);
                _toDispose.Add(fragmentPool);
            }

            allProjectileEvents = new AllProjectilesEvents(allProjectilesFactories);
            var fragmentsLauncher = new FragmentsLauncher(allProjectileEvents, fragmentsPools);
            var timerViewPool = new FollowingTimerViewPool(GameConfig.FollowingTimerViewPrefab, allProjectileEvents);
            var explosionPool = new ExplosionPool(GameConfig.ExplosionConfig, projectilesParent, shovel, allProjectileEvents);
            
            _toDispose.Add(timerViewPool);
            _toDispose.Add(explosionPool);
            _toDispose.Add(fragmentsLauncher);
            _toDispose.Add(allProjectileEvents);
        }

        private void SpawnWorms()
        {
            BattleSettings.GetSettings(out int teamsNumber, out int wormsNumber);
            
            var wormFactory = new WormFactory(GameConfig.WormPrefab, StartStateData.Terrain);
            var teamFactory = new TeamFactory(wormFactory);
            _wormInfoFactory = new WormInfoFactory(GameConfig.WormInfoViewPrefab, wormFactory);
            
            StartStateData.WormsSpawner.Init(teamFactory);
            TeamDiedEvent = teamFactory;
            
            CycledList<Team> teams = StartStateData.WormsSpawner.Spawn(teamsNumber, wormsNumber);

            TurnStateData.AliveTeams.AddRange(teams);

            TeamHealthFactory teamHealthFactory = Instantiate(GameConfig.TeamHealthFactoryPrefab, TeamHealthParent);
            teamHealthFactory.Create(TurnStateData.AliveTeams, GameConfig.TeamHealthPrefab);
        }

        private void CreateWeapon(List<ProjectilePool> projectilePools, out WeaponChanger weaponChanger)
        {
            WeaponFactory weaponFactory = new();
            WeaponSelectorItemFactory itemFactory = new();
            
            var weaponView = Instantiate(GameConfig.WeaponViewPrefab);
            weaponView.gameObject.SetActive(false);
            
            IEnumerable<Weapon> weaponList = weaponFactory.Create(GameConfig.WeaponConfigs);

            itemFactory.Create(weaponList, GameConfig.ItemPrefab, TurnStateData.WeaponSelector.ItemParent);
            _toDispose.Add(itemFactory);
            TurnStateData.WeaponSelector.Init(itemFactory);
            weaponView.Init(itemFactory);

            weaponChanger = new WeaponChanger(itemFactory, weaponFactory, weaponView);
            new ProjectileLauncher(itemFactory, weaponFactory, weaponView, projectilePools);
        }

        public void Exit()
        {
        }

        private void OnTeamDied(Team team)
        {
            TurnStateData.AliveTeams.Remove(team);

            if (TurnStateData.AliveTeams.Count <= 1)
            {
                TeamDiedEvent.TeamDied -= OnTeamDied;
                _stateSwitcher.SwitchState<BattleEndState>();
            }
        }

        public void Tick(){}

        public void FixedTick()
        {
        }

        public void HandleInput(){}
        
        public void Dispose()
        {
            foreach (var disposable in _toDispose) 
                disposable.Dispose();
        }
    }
}