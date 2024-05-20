using System;
using System.Collections.Generic;
using System.Linq;
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
using Wind;
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
        [SerializeField] private ProjectilePool _fragmentsPool;
        
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
        [SerializeField] private FollowingObject _followingTimerViewPrefab;
        [SerializeField] private Worm _wormPrefab;
        [SerializeField] private Arrow _arrowPrefab;
        [SerializeField] private Shovel _shovelPrefab;

        [NonSerialized] public Worm CurrentWorm;
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
        public readonly CycledList<Worm> WormsList = new();
        public readonly CycledList<Team> TeamsList = new();
        private readonly WeaponFactory _weaponFactory = new();

        public Wind.Wind Wind;
        public PlayerInput PlayerInput;
        private WormFactory _wormFactory;
        private TeamFactory _teamFactory;
        private List<Weapon> _weaponList;
        private ShovelWrapper _shovelWrapper;
        
        public void Init(MainInput mainInput)
        {
            PlayerInput = new PlayerInput(mainInput, FollowingCamera);
            
            Arrow = Object.Instantiate(_arrowPrefab);
            
            Shovel shovel = Object.Instantiate(_shovelPrefab);
            _shovelWrapper = new ShovelWrapper(shovel);
            
            InitializeWind();

            InitializePools();
            
            CreateWeapon();

            InitializeWorms();
            SpawnWorms();
            
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
            Wind = new Wind.Wind(WindData);
            WindEffect.Init(Wind);
            WindView.Init(Wind);
            WindMediator = new WindMediator(Wind, WeaponConfigs.Select(config => config.ProjectilePool));
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

        public void Tick()
        {
            GlobalTimer.Tick();
            TurnTimer.Tick();
        }

        public void FixedTick()
        {
            WindMediator.FixedTick();
        }
        
        private void InitializePools()
        {
            _explosionPool.Init(_projectilesParent, _shovelWrapper);
            _fragmentsPool.Init(_followingTimerViewPrefab);

            foreach (var weaponConfig in WeaponConfigs)
            {
                ProjectilePool projectilePool = weaponConfig.ProjectilePool;
            
                projectilePool.Init(_followingTimerViewPrefab);
                projectilePool.ProjectileFactory.Init(_projectilesParent);
            }
        }
        
        private void SpawnWorms()
        {
            _wormsSpawner.Init(_teamFactory);
            _wormsSpawner.Spawn(out var worms, out var teams);
            
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
