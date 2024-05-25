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
using static UnityEngine.Object;

namespace BattleStateMachineComponents
{
    [Serializable]
    public class BattleStateMachineData
    {
        [SerializeField] private WormsSpawner _wormsSpawner;
        [field: SerializeField] public FollowingCamera FollowingCamera { get; private set; }
        [field: SerializeField] public Water Water { get; private set; }

        [field: Header("Factories")]
        [SerializeField] private TeamHealthFactory _teamHealthFactory;

        [field: Header("UI")]
        [field: SerializeField] public WeaponSelector WeaponSelector { get; private set; }
        [field: SerializeField] public TimerView TurnTimerView { get; private set; }
        [field: SerializeField] public TimerView GlobalTimerView { get; private set; }
        [field: SerializeField] public EndScreen EndScreen { get; private set; }
        [field: SerializeField] public WindView WindView { get; private set; }
        [field: SerializeField] public GameConfig GameConfig { get; private set; }

        [NonSerialized] public IWorm CurrentWorm;
        [NonSerialized] public Team CurrentTeam;

        public WeaponView WeaponView { get; private set; }
        public Arrow Arrow { get; private set; }
        public CycledList<Team> AliveTeams { get; private set; } = new();
        public WeaponChanger WeaponChanger { get; private set; }
        public AllProjectilesEventProvider AllProjectileEvents { get; private set; }
        public WindMediator WindMediator { get; private set; }

        public readonly Timer TurnTimer = new();
        public readonly Timer GlobalTimer = new();
        public readonly CycledList<IWorm> WormsList = new();
        public readonly CycledList<Team> TeamsList = new();

        public PlayerInput PlayerInput;
        private ShovelWrapper _shovelWrapper;
        private BattleSettings _battleSettings = new();
        
        private WormFactory _wormFactory;
        private TeamFactory _teamFactory;
        private readonly WeaponFactory _weaponFactory = new();
        private List<ProjectileFactory> _projectileFactories = new();
        private WeaponSelectorItemFactory _itemFactory = new();
        private WormInfoFactory _wormInfoFactory;
        
        private ExplosionPool _explosionPool;
        private FollowingTimerViewPool _followingTimerViewPool;
        private Dictionary<ProjectileConfig, ProjectilePool> _projectilePools = new();
        
        public void Init(MainInput mainInput)
        {
            Water.Init(GameConfig.WaterStep);
            _battleSettings.GetSettings(out int teamsNumber, out int wormsNumber);

            PlayerInput = new PlayerInput(mainInput, FollowingCamera, WeaponSelector);

            Arrow = Instantiate(GameConfig.ArrowPrefab);

            Shovel shovel = Instantiate(GameConfig.ShovelPrefab);
            _shovelWrapper = new ShovelWrapper(shovel);
            
            CreateWeapon();

            InitializeWorms();
            InitializePools();

            WindMediator = new WindMediator(GameConfig.WindData, WindView, AllProjectileEvents);
            SpawnWorms(teamsNumber, wormsNumber);

            GlobalTimerView.Init(GlobalTimer, TimerFormattingStyle.MinutesAndSeconds);
            TurnTimerView.Init(TurnTimer, TimerFormattingStyle.Seconds);
        }

        public void Dispose()
        {
            _teamFactory.TeamDied -= OnTeamDied;
            _itemFactory.Dispose();
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
            var projectilesParent = Instantiate(new GameObject()).transform;
            
            _followingTimerViewPool = new FollowingTimerViewPool(GameConfig.FollowingTimerViewPrefab);

            foreach (var weaponConfig in GameConfig.WeaponConfigs)
            {
                var configurator = new ProjectileConfigurator(_followingTimerViewPool);
                var factory = new ProjectileFactory(weaponConfig.ProjectileConfig, projectilesParent, configurator);
                
                _projectileFactories.Add(factory);

                var pool = new ProjectilePool(factory, 1);

                _projectilePools.Add(weaponConfig.ProjectileConfig, pool);
            }

            AllProjectileEvents = new AllProjectilesEventProvider(_projectileFactories);
            _explosionPool = new ExplosionPool(projectilesParent, _shovelWrapper, AllProjectileEvents);
        }

        private void InitializeWorms()
        {
            _wormFactory = new WormFactory(GameConfig.WormPrefab);
            _teamFactory = new TeamFactory(_wormFactory);
            _wormInfoFactory = new WormInfoFactory(GameConfig.WormInfoViewPrefab, _wormFactory);

            _teamFactory.TeamDied += OnTeamDied;
        }

        private void SpawnWorms(int teamsNumber, int wormsNumber)
        {
            _wormsSpawner.Init(_teamFactory);
            _wormsSpawner.Spawn(teamsNumber, wormsNumber, out var worms, out var teams);

            WormsList.AddRange(worms);
            TeamsList.AddRange(teams);

            foreach (var worm in WormsList)
                worm.SetRigidbodyKinematic();

            _teamHealthFactory.Create(TeamsList, GameConfig.TeamHealthPrefab);
            
            AliveTeams.AddRange(TeamsList);
        }

        private void CreateWeapon()
        {
            WeaponView = Instantiate(GameConfig.WeaponViewPrefab);
            WeaponView.gameObject.SetActive(false);

            IEnumerable<Weapon> weaponList = _weaponFactory.Create(GameConfig.WeaponConfigs);

            _itemFactory.Create(weaponList, _projectilePools, GameConfig.ItemPrefab, WeaponSelector.ItemParent);
            WeaponSelector.Init(_itemFactory);
            WeaponView.Init(_itemFactory);

            WeaponChanger = new WeaponChanger(_itemFactory, _weaponFactory, WeaponView);
            new ProjectileLauncher(_itemFactory, _weaponFactory, WeaponView);
        }
    }
}