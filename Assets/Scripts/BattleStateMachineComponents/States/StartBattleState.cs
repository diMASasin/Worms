using System;
using System.Collections.Generic;
using Battle_;
using BattleStateMachineComponents.StatesData;
using Configs;
using EventProviders;
using Factories;
using InputService;
using Pools;
using Projectiles;
using Timers;
using UnityEngine;
using Weapons;
using WormComponents;
using static UnityEngine.Object;

namespace BattleStateMachineComponents.States
{
    public class StartBattleState : IBattleState
    {
        private readonly Timer _globalTimer = new();
        private readonly List<IDisposable> _toDispose = new ();
        private readonly List<ProjectileFactory> _projectileFactories = new();

        private Transform TeamHealthParent => _startStateData.UIChanger.transform;
        private TimersConfig TimersConfig => _data.TimersConfig;
        private GameConfig GameConfig => _startStateData.GameConfig;
        private float WaterStep => GameConfig.WaterStep;
        private ITeamDiedEventProvider TeamDiedEvent { get; set; }

        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
        private readonly StartStateData _startStateData;
        private readonly TurnStateData _turnStateData;
        private readonly BetweenTurnsStateData _betweenTurnsData;

        public StartBattleState(IStateSwitcher stateSwitcher, BattleStateMachineData data,
            StartStateData startStateData, TurnStateData turnStateData, BetweenTurnsStateData betweenTurnsData)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
            _startStateData = startStateData;
            _turnStateData = turnStateData;
            _betweenTurnsData = betweenTurnsData;
        }

        public void Enter()
        {
            _data.PlayerInput = new PlayerInput(_data.MainInput, _data.FollowingCamera, _turnStateData.WeaponSelector);
            
            _startStateData.Terrain.GetEdgesForSpawn();
            _globalTimer.Start(TimersConfig.GlobalTime, () => _betweenTurnsData.Water.AllowIncreaseWaterLevel());
            
            Arrow arrow = Instantiate(GameConfig.ArrowPrefab);
            ShovelWrapper shovelWrapper = new (Instantiate(GameConfig.ShovelPrefab));

            Dictionary<ProjectileConfig, ProjectilePool> projectilePools;
            
            InitializePools(shovelWrapper, out var allProjectileEvents, out projectilePools);
            CreateWeapon(projectilePools, out WeaponChanger weaponChanger);

            _betweenTurnsData.Init(GameConfig.WindData, _startStateData.WindView, allProjectileEvents, WaterStep);
            
            SpawnWorms();

            _startStateData.GlobalTimerView.Init(_globalTimer, TimerFormattingStyle.MinutesAndSeconds);
            _startStateData.TurnTimerView.Init(_data.TurnTimer, TimerFormattingStyle.Seconds);
            
            _turnStateData.Init(arrow, allProjectileEvents, weaponChanger);
            
            TeamDiedEvent.TeamDied += OnTeamDied;
            _stateSwitcher.SwitchState<BetweenTurnsState>();
        }

        private void InitializePools(ShovelWrapper shovel, out AllProjectilesEvents allProjectileEvents, 
            out Dictionary<ProjectileConfig, ProjectilePool> projectilePools)
        {
            var projectilesParent = Instantiate(new GameObject()).transform;
            
            var followingTimerViewPool = new FollowingTimerViewPool(GameConfig.FollowingTimerViewPrefab);
            projectilePools = new Dictionary<ProjectileConfig, ProjectilePool>();

            foreach (var weaponConfig in GameConfig.WeaponConfigs)
            {
                var configurator = new ProjectileConfigurator(followingTimerViewPool);
                var factory = new ProjectileFactory(weaponConfig.ProjectileConfig, projectilesParent, configurator);
                
                _projectileFactories.Add(factory);

                var pool = new ProjectilePool(factory, 1);

                projectilePools.Add(weaponConfig.ProjectileConfig, pool);
            }

            allProjectileEvents = new AllProjectilesEvents(_projectileFactories);
            new ExplosionPool(projectilesParent, shovel, allProjectileEvents);
        }

        private void SpawnWorms()
        {
            BattleSettings.GetSettings(out int teamsNumber, out int wormsNumber);
            
            var wormFactory = new WormFactory(GameConfig.WormPrefab, _startStateData.Terrain);
            var teamFactory = new TeamFactory(wormFactory);
            new WormInfoFactory(GameConfig.WormInfoViewPrefab, wormFactory);
            
            _startStateData.WormsSpawner.Init(teamFactory);
            TeamDiedEvent = teamFactory;
            
            CycledList<Team> teams = _startStateData.WormsSpawner.Spawn(teamsNumber, wormsNumber);

            _turnStateData.AliveTeams.AddRange(teams);

            TeamHealthFactory teamHealthFactory = Instantiate(GameConfig.TeamHealthFactoryPrefab, TeamHealthParent);
            teamHealthFactory.Create(_turnStateData.AliveTeams, GameConfig.TeamHealthPrefab);
        }

        private void CreateWeapon(Dictionary<ProjectileConfig, ProjectilePool> projectilePools, 
            out WeaponChanger weaponChanger)
        {
            WeaponFactory weaponFactory = new();
            WeaponSelectorItemFactory itemFactory = new();
            
            var weaponView = Instantiate(GameConfig.WeaponViewPrefab);
            weaponView.gameObject.SetActive(false);
            
            IEnumerable<Weapon> weaponList = weaponFactory.Create(GameConfig.WeaponConfigs);

            itemFactory.Create(weaponList, GameConfig.ItemPrefab, _turnStateData.WeaponSelector.ItemParent);
            _toDispose.Add(itemFactory);
            _turnStateData.WeaponSelector.Init(itemFactory);
            weaponView.Init(itemFactory);

            weaponChanger = new WeaponChanger(itemFactory, weaponFactory, weaponView);
            new ProjectileLauncher(itemFactory, weaponFactory, weaponView, projectilePools);
        }

        public void Exit()
        {
        }

        private void OnTeamDied(Team team)
        {
            _turnStateData.AliveTeams.Remove(team);

            if (_turnStateData.AliveTeams.Count <= 1)
            {
                TeamDiedEvent.TeamDied -= OnTeamDied;
                _stateSwitcher.SwitchState<BattleEndState>();
            }
        }

        public void Tick(){}

        public void HandleInput(){}
        
        public void FixedTick()
        {
            _betweenTurnsData.WindMediator.FixedTick();

            foreach (var projectileFactory in _projectileFactories)
                projectileFactory.FixedTick();
        }

        public void OnDrawGizmos()
        {
            foreach (var factory in _projectileFactories)
                factory.OnDrawGizmos();
        }
        
        public void Dispose()
        {
            foreach (var disposable in _toDispose) 
                disposable.Dispose();
        }
    }
}