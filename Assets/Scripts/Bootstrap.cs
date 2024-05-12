using System.Collections.Generic;
using System.Linq;
using Configs;
using DefaultNamespace.Wind;
using Factories;
using Pools;
using ScriptBoy.Digable2DTerrain;
using Timers;
using UnityEngine;
using Weapons;

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
    [SerializeField] private float _turnDelay = 2.5f;
    [SerializeField] private TimersConfig _timersConfig;
    [SerializeField] private WindData _windData;
    [SerializeField] private WindEffect _windEffect;
    [SerializeField] private WindView _windView;
    [SerializeField] private TeamHealthFactory _teamHealthFactory;
    [SerializeField] private ExplosionPool _explosionPool;
    [SerializeField] private CoroutinePerformer _coroutinePerformer;
    [SerializeField] private WeaponView _weaponView;
    [SerializeField] private WeaponFactory _weaponFactory;
    [SerializeField] private FollowingObject _followingTimerViewPrefab;
    [SerializeField] private Worm _wormPrefab;
    [SerializeField] private ProjectileFactory _projectileFactory;

    private TeamFactory _teamFactory;
    private WormFactory _wormFactory;
    private Wind _wind;
    private ShovelWrapper _shovelWrapper;
    private WaterMediator _waterMediator;
    private WindMediator _windMediator;
    private WeaponPresenter _weaponPresenter;
    
    private Game _game;
    private List<Team> _teams;
    private List<Worm> _worms;

    private readonly List<Team> _currentTeams = new();
    private List<Weapon> _weaponList = new();
    private TimerMediator _timerMediator;
    private Timer _globalTimer;
    private Timer _turnTimer;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _coroutinePerformer.Init();

        _shovelWrapper = new ShovelWrapper(_shovel);
        
        _wind = new Wind(_windData);
        _windEffect.Init(_wind);
        _windView.Init(_wind);

        InitializePools();
        
        Spawn();
        _game = new Game(_teams, _teamFactory, _timersConfig);
        _wormFactory = new WormFactory(_wormPrefab);
        _teamFactory = new TeamFactory(_wormFactory);

        CreateWeapon();
        _weaponSelector.Init(_weaponList, _weaponFactory, _game);

        _globalTimer = new Timer();
        _turnTimer = new Timer();
        _globalTimerView.Init(_globalTimer, TimerFormattingStyle.MinutesAndSeconds);
        _timerView.Init(_turnTimer, TimerFormattingStyle.Seconds);

        _timerMediator = new TimerMediator(_globalTimer, _turnTimer, _timersConfig, _game, _weaponFactory);
        _waterMediator = new WaterMediator(_water, _timerMediator, _game);
        _windMediator = new WindMediator(_wind, _game, _weaponConfigs.Select(config => config.ProjectilePool));
        _playerInput = new PlayerInput(_game);
        
        _followingCamera.Init(_game, _projectileFactory, _wormFactory);
        
        StartGame();
    }

    private void OnEnable()
    {
        _game.GameEnd += OnGameEnd;
        _teamFactory.TeamDied += OnTeamDied;
        _wormFactory.WormDied += OnWormDied;
    }

    private void OnDisable()
    {
        if (_game != null)
            _game.GameEnd -= OnGameEnd;

        if (_teamFactory != null) 
            _teamFactory.TeamDied -= OnTeamDied;

        if(_wormFactory != null)
            _wormFactory.WormDied -= OnWormDied;
    }

    private void OnGameEnd()
    {
        _endScreen.gameObject.SetActive(true);
    }

    private void StartGame()
    {
        _game.StartGame();
        _game.StartNextTurn();
    }

    private void Spawn()
    {
        _wormsSpawner.Init(_teamFactory);
        _wormsSpawner.Spawn(out List<Worm> worms, out List<Team> teams);
        
        _worms = worms;
        _teams = teams;
        
        _currentTeams.AddRange(_teams);
        _teamHealthFactory.Create(_teams);
    }

    private void Update()
    {
        _globalTimer.Tick();
        _turnTimer.Tick();
        _playerInput.Tick();
        _game.Tick();
    }

    private void FixedUpdate()
    {
        _windMediator.FixedTick();
    }

    private void OnDestroy()
    {
        if (_game != null)
        {
            _game.Dispose();
            _waterMediator.Dispose();
            _windMediator.Dispose();
            _timerMediator.Dispose();
        }
    }

    private void OnTeamDied(Team team)
    {
        _currentTeams.Remove(team); 
    }

    private void OnWormDied(Worm worm)
    {
        if (_game.CurrentWorm == worm)
            _game.StartNextTurn(_turnDelay);
    }

    private void InitializePools()
    {
        _explosionPool.Init(_projectilesParent, _shovelWrapper);
        _fragmentsPool.Init(_followingTimerViewPrefab);

        foreach (var weaponConfig in _weaponConfigs)
            weaponConfig.ProjectilePool.Init(_followingTimerViewPrefab);
    }

    private void CreateWeapon()
    {
        _weaponList = _weaponFactory.Create(_weaponConfigs, _weaponView.SpawnPoint);

        _weaponPresenter = new WeaponPresenter(_weaponView, _weaponSelector);
    }
}
