using System;
using System.Collections.Generic;
using System.Linq;
using Battle_;
using CameraFollow;
using Configs;
using Factories;
using InputService;
using Pools;
using Projectiles;
using ScriptBoy.Digable2DTerrain.Scripts;
using Spawn;
using Timers;
using UI;
using UnityEngine;
using Weapons;
using Wind_;
using WormComponents;
using Object = UnityEngine.Object;

namespace BattleStateMachineComponents
{
    [Serializable]
    public class BattleStateMachineData
    {
        [field: SerializeField] public FollowingCamera FollowingCamera { get; private set; }
        [SerializeField] private WormsSpawner _wormsSpawner;
        [SerializeField] private Water _water;
        [field: Header("Wind")]
        [field: SerializeField] public WindData WindData { get; private set; }
        [field: SerializeField] public WindEffect WindEffect { get; private set; }
        [field: Header("Projectiles")]
        [SerializeField] private Transform _projectilesParent;
        
        [field: Header("Factories")]
        [SerializeField] private WormInfoFactory _wormInfoFactory;
        [SerializeField] private TeamHealthFactory _teamHealthFactory;
        
        [field: Header("Pools")]
        [SerializeField] private ExplosionPool _explosionPool;
        
        [field: Header("UI")]
        [field: SerializeField] public WeaponSelector WeaponSelector { get; private set; }
        [field: SerializeField] public TimerView TurnTimerView { get; private set; }
        [field: SerializeField] public TimerView GlobalTimerView { get; private set; }
        [field: SerializeField] public EndScreen EndScreen { get; private set; }
        [field: SerializeField] public WindView WindView { get; private set; }
        
        [field: Header("Configs"), SerializeField]
        public WeaponConfig[] WeaponConfigs { get; private set; }
        [field: SerializeField] public TimersConfig TimersConfig { get; private set; }

        [field: Header("Prefabs"), SerializeField]
        public WeaponView WeaponViewPrefab { get; private set; }
        [SerializeField] private FollowingTimerView _followingTimerViewPrefab;
        [SerializeField] private Worm _wormPrefab;
        [SerializeField] private Arrow _arrowPrefab;
        [SerializeField] private Shovel _shovelPrefab;

        [NonSerialized] public IWorm CurrentWorm;
        [NonSerialized] public Team CurrentTeam;
        
        public WaterMediator WaterMediator { get; private set; }
        public WeaponView WeaponView { get; private set; }
        public Arrow Arrow { get; private set; }
        public WindMediator WindMediator { get; private set; }
        public CycledList<Team> AliveTeams { get; private set; } = new();
        public WeaponChanger WeaponChanger { get; private set; }
        public ProjectileLauncher ProjectileLauncher { get; private set; }
        
        public readonly Timer TurnTimer = new();
        public readonly Timer GlobalTimer = new();
        public readonly CycledList<IWorm> WormsList = new();
        public readonly CycledList<Team> TeamsList = new();
        private readonly WeaponFactory _weaponFactory = new();

        public Wind Wind;
        public PlayerInput PlayerInput;
        private WormFactory _wormFactory;
        private TeamFactory _teamFactory;
        private List<Weapon> _weaponList;
        private ShovelWrapper _shovelWrapper;
        private BattleSettings _battleSettings = new();
        private IEnumerable<ProjectileFactory> _projectileFactories;
        private FollowingTimerViewPool _followingTimerViewPool;

        public void Init(MainInput mainInput)
        {
            _battleSettings.GetSettings(out int teamsNumber, out int wormsNumber);
            
            PlayerInput = new PlayerInput(mainInput, FollowingCamera, WeaponSelector);
            
            Arrow = Object.Instantiate(_arrowPrefab);
            
            Shovel shovel = Object.Instantiate(_shovelPrefab);
            _shovelWrapper = new ShovelWrapper(shovel);
            
            InitializeWind();

            InitializePools();
            
            CreateWeapon();

            InitializeWorms();
            SpawnWorms(teamsNumber, wormsNumber);
            
            WaterMediator = new WaterMediator(_water);
            
            GlobalTimerView.Init(GlobalTimer, TimerFormattingStyle.MinutesAndSeconds);
            TurnTimerView.Init(TurnTimer, TimerFormattingStyle.Seconds);
        }

        private void InitializeWorms()
        {
            _wormFactory = new WormFactory(_wormPrefab);
            _wormInfoFactory.Init(_wormFactory);
            _teamFactory = new TeamFactory(_wormFactory);
            
            _teamFactory.TeamDied += OnTeamDied;
        }

        private void InitializeWind()
        {
            Wind = new Wind(WindData);
            WindEffect.Init(Wind);
            WindView.Init(Wind);
            WindMediator = new WindMediator(Wind);
        }

        public void Dispose()
        {
            _teamFactory.TeamDied -= OnTeamDied;
        }

        private void OnTeamDied(Team team)
        {
            AliveTeams.Remove(team);
            
            if (AliveTeams.Count <= 1) 
                EndScreen.Show();
        }

        public void FixedTick()
        {
            WindMediator.FixedTick();

            foreach (var projectileFactory in _projectileFactories) 
                projectileFactory.FixedTick();
        }

        public void OnDrawGizmos()
        {
            foreach (var factory in _projectileFactories) 
                factory.OnDrawGizmos();
        }
        
        private void InitializePools()
        {
            _explosionPool.Init(_projectilesParent, _shovelWrapper);

            _projectileFactories = WeaponConfigs.Select(config => config.ProjectilePool.ProjectileFactory);

            _followingTimerViewPool = new FollowingTimerViewPool(_followingTimerViewPrefab);
            
            foreach (var projectileFactory in _projectileFactories)
                projectileFactory.Init(_projectilesParent, _followingTimerViewPool);
        }
        
        private void SpawnWorms(int teamsNumber, int wormsNumber)
        {
            _wormsSpawner.Init(_teamFactory);
            _wormsSpawner.Spawn(teamsNumber, wormsNumber, out var worms, out var teams);
            
            WormsList.AddRange(worms);
            TeamsList.AddRange(teams);

            foreach (var worm in WormsList) 
                worm.SetRigidbodyKinematic();

            _teamHealthFactory.Create(TeamsList);
            
            AliveTeams.AddRange(TeamsList);
        }
        
        private void CreateWeapon()
        {
            WeaponView = Object.Instantiate(WeaponViewPrefab);
            WeaponView.gameObject.SetActive(false);
            
            _weaponList = _weaponFactory.Create(WeaponConfigs, WeaponView.SpawnPoint);
            WeaponSelector.Init(_weaponList);
            WeaponView.Init(WeaponSelector);
            
            WeaponChanger = new WeaponChanger(WeaponSelector, _weaponFactory, WeaponView);
            ProjectileLauncher = new ProjectileLauncher(WeaponSelector, _weaponFactory, WeaponView);
        }
    }
}
