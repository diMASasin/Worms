using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using DefaultNamespace.Wind;
using EventProviders;
using Factories;
using PlayerInputSystem;
using Pools;
using Projectiles;
using ScriptBoy.Digable2DTerrain;
using Timers;
using UnityEngine;
using Weapons;
using WormComponents;
using Object = UnityEngine.Object;

namespace GameBattleStateMachine
{
    [Serializable]
    public class BattleStateMachineData
    {
        [field: SerializeField] public FollowingCamera FollowingCamera { get; private set; }
        [field: SerializeField] public Vector3 GeneralViewPosition { get; private set; }
        [field: SerializeField] public EndScreen EndScreen { get; private set; }
        [SerializeField] private Shovel _shovelPrefab;
        [SerializeField] private WormsSpawner _wormsSpawner;
        [SerializeField] private Water _water;
        [field: Header("TimerViews")]
        [field: SerializeField] public TimerView TurnTimerView { get; private set; }
        [field: SerializeField] public TimerView GlobalTimerView { get; private set; }
        [field: Header("Wind")]
        [field: SerializeField] public WindData WindData { get; private set; }
        [field: SerializeField] public WindEffect WindEffect { get; private set; }
        [field: SerializeField] public WindView WindView { get; private set; }
        [field: Header("Projectiles")]
        [SerializeField] private Transform _projectilesParent;
        
        [field: Header("Factories")]
        [SerializeField] private WormInfoFactory _wormInfoFactory;
        [SerializeField] private TeamHealthFactory _teamHealthFactory;
        
        [field: Header("Pools")]
        [SerializeField] private ExplosionPool _explosionPool;
        [SerializeField] private ProjectilePool _fragmentsPool;
        
        [field: Header("Weapons")]
        [field: SerializeField] public WeaponSelector WeaponSelector { get; private set; }
        
        [field: Header("Configs"), SerializeField]
        public WeaponConfig[] WeaponConfigs { get; private set; }
        [field: SerializeField] public TimersConfig TimersConfig { get; private set; }

        [field: Header("Prefabs"), SerializeField]
        public WeaponView WeaponViewPrefab { get; private set; }
        [SerializeField] private FollowingObject _followingTimerViewPrefab;
        [SerializeField] private Worm _wormPrefab;
        [SerializeField] private Arrow _arrowPrefab;
        
        [NonSerialized] public int CurrentTeamIndex = -1;
        [NonSerialized] public Worm CurrentWorm;
        [NonSerialized] public Team CurrentTeam;

        public readonly CycledList<Worm> WormsList = new();
        public readonly CycledList<Team> TeamsList = new();
        public readonly Timer GlobalTimer = new Timer();
        public readonly Timer TurnTimer = new Timer();
        private readonly WeaponFactory _weaponFactory = new();

        public WaterMediator WaterMediator { get; private set; }
        public WeaponView WeaponView { get; private set; }
        public Arrow Arrow { get; private set; }
        public WindMediator WindMediator { get; private set; }
        public CycledList<Team> AliveTeams { get; private set; } = new();
        public WeaponChanger WeaponChanger { get; private set; }
        public ProjectileLauncher ProjectileLauncher { get; private set; }

        public PlayerInput PlayerInput;
        public Wind Wind;
        private ShovelWrapper _shovelWrapper;
        private WormFactory _wormFactory;
        private TeamFactory _teamFactory;
        private List<Weapon> _weaponList;
        
        public void Init(PlayerInput playerInput)
        {
            PlayerInput = playerInput;
            
            WeaponView = Object.Instantiate(WeaponViewPrefab);
            Arrow = Object.Instantiate(_arrowPrefab);
            Shovel shovel = Object.Instantiate(_shovelPrefab);
            
            WeaponView.gameObject.SetActive(false);
            
            _shovelWrapper = new ShovelWrapper(shovel);
            
            InitializeWind();

            InitializePools();
            ProjectileLauncher = new ProjectileLauncher(WeaponSelector, _weaponFactory, WeaponView);
            
            _wormFactory = new WormFactory(_wormPrefab);
            _wormInfoFactory.Init(_wormFactory);
            _teamFactory = new TeamFactory(_wormFactory);
            _teamFactory.TeamDied += OnTeamDied;
            
            CreateWeapon();
            Spawn();
            
            WaterMediator = new WaterMediator(_water);
            
            GlobalTimerView.Init(GlobalTimer, TimerFormattingStyle.MinutesAndSeconds);
            TurnTimerView.Init(TurnTimer, TimerFormattingStyle.Seconds);
        }

        private void InitializeWind()
        {
            Wind = new Wind(WindData);
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
        
        public bool TryGetNextTeam(out Team team)
        {
            team = TeamsList.Next();
            return team != null;
        }

        public void Tick()
        {
            PlayerInput.Tick();
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
        
        private void Spawn()
        {
            _wormsSpawner.Init(_teamFactory);
            _wormsSpawner.Spawn(out var worms, out var teams);
            
            WormsList.AddRange(worms);
            TeamsList.AddRange(teams);
        
            _teamHealthFactory.Create(TeamsList);
            AliveTeams.AddRange(TeamsList);
        }
        
        private void CreateWeapon()
        {
            _weaponList = _weaponFactory.Create(WeaponConfigs, WeaponView.SpawnPoint);
            WeaponSelector.Init(_weaponList);
            WeaponView.Init(WeaponSelector);
            WeaponChanger = new WeaponChanger(WeaponSelector, _weaponFactory, WeaponView);
        }
    }
}