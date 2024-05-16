using System.Collections.Generic;
using System.Linq;
using Configs;
using DefaultNamespace.Wind;
using Factories;
using GameBattleStateMachine;
using GameStateMachine;
using Pools;
using Projectiles;
using ScriptBoy.Digable2DTerrain;
using Timers;
using UnityEngine;
using Weapons;
using Input = PlayerInput.Input;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private WeaponConfig[] _weaponConfigs;
    [SerializeField] private WeaponSelector _weaponSelector;
    [SerializeField] private FollowingCamera _followingCamera;
    [SerializeField] private TimerView _globalTimerView;
    [SerializeField] private TimerView _timerView;
    [SerializeField] private Water _water;
    [SerializeField] private Shovel _shovel;
    [SerializeField] private Transform _projectilesParent;
    [SerializeField] private ProjectilePool _fragmentsPool;
    [SerializeField] private WormsSpawner _wormsSpawner;
    [SerializeField] private EndScreen _endScreen;
    [SerializeField] private TimersConfig _timersConfig;
    [SerializeField] private WindData _windData;
    [SerializeField] private WindEffect _windEffect;
    [SerializeField] private WindView _windView;
    [SerializeField] private TeamHealthFactory _teamHealthFactory;
    [SerializeField] private ExplosionPool _explosionPool;
    [SerializeField] private CoroutinePerformer _coroutinePerformer;
    [SerializeField] private WeaponView _weaponViewPrefab;
    [SerializeField] private FollowingObject _followingTimerViewPrefab;
    [SerializeField] private Worm _wormPrefab;
    [SerializeField] private WormInfoFactory _wormInfoFactory;
    [SerializeField] private Vector3 _generalViewPosition = new(0, 4.7f, -40);
    [SerializeField] private Arrow _arrowPrefab;
    
    private readonly WeaponFactory _weaponFactory = new();
    private readonly List<Team> _aliveTeams = new();
    private readonly Timer _globalTimer = new Timer();
    private readonly Timer _turnTimer = new Timer();
    private MainInput _mainInput;
    private List<Weapon> _weaponList = new();
    private Input _input;
    
    private TeamFactory _teamFactory;
    private WormFactory _wormFactory;
    private Wind _wind;
    private ShovelWrapper _shovelWrapper;
    private WaterMediator _waterMediator;
    private WindMediator _windMediator;
    
    private Game _game;
    private List<Team> _teams;
    private List<Worm> _worms;
    private Arrow _arrow;
    private ProjectileLauncher _projectileLauncher;
    private WeaponChanger _weaponChanger;
    private WeaponView _weaponView;

    private void Awake()
    {
        _arrow = Instantiate(_arrowPrefab);
        _weaponView = Instantiate(_weaponViewPrefab);
        _weaponView.gameObject.SetActive(false);
        _coroutinePerformer.Init();
        _mainInput = new MainInput();
        _input = new Input(_mainInput);

        _shovelWrapper = new ShovelWrapper(_shovel);
        
        _wind = new Wind(_windData);
        _windEffect.Init(_wind);
        _windView.Init(_wind);

        InitializePools();
        _projectileLauncher = new ProjectileLauncher(_weaponSelector, _weaponFactory, _weaponView);
        
        _wormFactory = new WormFactory(_wormPrefab);
        _wormInfoFactory.Init(_wormFactory);
        _teamFactory = new TeamFactory(_wormFactory);
        Spawn();
        
        CreateWeapon();
        
        _globalTimerView.Init(_globalTimer, TimerFormattingStyle.MinutesAndSeconds);
        _timerView.Init(_turnTimer, TimerFormattingStyle.Seconds);

        _waterMediator = new WaterMediator(_water);
        _windMediator = new WindMediator(_wind, _weaponConfigs.Select(config => config.ProjectilePool));
        
        BattleStateMachineData battleStateMachineData = new (_timersConfig, _followingCamera, _endScreen, _input, 
            _generalViewPosition, _turnTimer, _globalTimer, _aliveTeams, _arrow, _weaponSelector, _weaponView, _wind,
            _weaponChanger, _waterMediator, _projectileLauncher, _windMediator);
        _game = new Game(_aliveTeams, _teamFactory, _endScreen, battleStateMachineData, _weaponFactory);
        
        _game.StartGame();
    }

    private void Spawn()
    {
        _wormsSpawner.Init(_teamFactory);
        _wormsSpawner.Spawn(out List<Worm> worms, out List<Team> teams);
        
        _worms = worms;
        _teams = teams;
        
        _teamHealthFactory.Create(_teams);
        _aliveTeams.AddRange(_teams);
    }

    private void Update()
    {
        _globalTimer.Tick();
        _turnTimer.Tick();
        _input.Tick();
        _game.Tick();
    }

    private void FixedUpdate()
    {
        _windMediator.FixedTick();
    }

    private void OnDestroy()
    {
        if (_game != null) _game.Dispose();
        
        _projectileLauncher.Dispose();
    }

    private void InitializePools()
    {
        _explosionPool.Init(_projectilesParent, _shovelWrapper);
        _fragmentsPool.Init(_followingTimerViewPrefab);

        foreach (var weaponConfig in _weaponConfigs)
        {
            ProjectilePool projectilePool = weaponConfig.ProjectilePool;
            
            projectilePool.Init(_followingTimerViewPrefab);
            projectilePool.ProjectileFactory.Init(_projectilesParent);
        }
    }

    private void CreateWeapon()
    {
        _weaponList = _weaponFactory.Create(_weaponConfigs, _weaponView.SpawnPoint);
        _weaponSelector.Init(_weaponList);
        _weaponView.Init(_weaponSelector);
        _weaponChanger = new WeaponChanger(_weaponSelector, _weaponFactory, _weaponView);
    }
}
