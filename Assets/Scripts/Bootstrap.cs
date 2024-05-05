using System.Collections.Generic;
using System.Linq;
using Configs;
using DefaultNamespace.Wind;
using Pools;
using Projectiles;
using ScriptBoy.Digable2DTerrain;
using Timers;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] private TeamHealthView _teamHealthView;
    [SerializeField] private ExplosionPool _explosionPool;
    [SerializeField] private CoroutinePerformer _coroutinePerformer;
    [SerializeField] private ProjectileViewPool _projectileViewPool;

    private Wind _wind;
    private ShovelWrapper _shovelWrapper;
    private WaterMediator _waterMediator;
    private WindMediator _windMediator;
    
    private Game _game;
    private List<Team> _teams;
    private List<Worm> _worms;

    private readonly List<Team> _currentTeams = new();
    private readonly List<Weapon> _weaponList = new();
    private TimerMediator _timerMediator;
    private Timer _globalTimer;
    private Timer _turnTimer;
    private PlayerInput _playerInput;

    public event UnityAction<List<Team>> WormsSpawned;

    private void Awake()
    {
        _coroutinePerformer.Init();

        _shovelWrapper = new ShovelWrapper(_shovel);
        _teamHealthView.Init(this);
        
        _wind = new Wind(_windData);
        _windEffect.Init(_wind);
        _windView.Init(_wind);

        InitializePools();
        CreateWeapon();
        Spawn();
        
        _game = new Game(_currentTeams);

        _globalTimer = new Timer();
        _turnTimer = new Timer();
        _globalTimerView.Init(_globalTimer, TimerFormattingStyle.MinutesAndSeconds);
        _timerView.Init(_turnTimer, TimerFormattingStyle.Seconds);

        _timerMediator = new TimerMediator(_globalTimer, _turnTimer, _timersConfig, _game, _weaponList);
        _waterMediator = new WaterMediator(_water, _timerMediator, _game);
        _windMediator = new WindMediator(_wind, _game, _weaponConfigs.Select(config => config.ProjectilePool));
        _playerInput = new PlayerInput(_game);

        _weaponSelector.Init(_weaponList, _game);

        _followingCamera.Init(_game, _worms, _projectileViewPool);
        
        StartGame();
    }

    private void OnEnable()
    {
        _game.GameEnd += OnGameEnd;

        foreach (var team in _teams)
            team.Died += OnTeamDied;

        foreach (var worm in _worms)
            worm.Died += OnWormDied;
    }

    private void OnDisable()
    {
        if (_game != null)
            _game.GameEnd -= OnGameEnd;

        if(_teams  != null)
            foreach (var team in _teams)
                team.Died -= OnTeamDied;

        if(_worms != null)
            foreach (var worm in _worms)
                worm.Died -= OnWormDied;
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
        _wormsSpawner.GetEdgesForSpawn();
        _teams = _wormsSpawner.SpawnTeams();
        _worms = _wormsSpawner.WormsList;
        WormsSpawned?.Invoke(_teams);
        
        _currentTeams.AddRange(_teams);
    }

    private void Update()
    {
        _globalTimer.Tick();
        _turnTimer.Tick();
        _playerInput.Tick();
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
        worm.Died -= OnWormDied;

        if (_game.CurrentWorm == worm)
            _game.StartNextTurn(_turnDelay);
    }

    private void InitializePools()
    {
        _explosionPool.Init(_projectilesParent, _shovelWrapper);

        _fragmentsPool.Init();
        _projectileViewPool.Init(_projectilesParent, _explosionPool);

        foreach (var weaponConfig in _weaponConfigs)
            weaponConfig.ProjectilePool.Init();
    }

    private void CreateWeapon()
    {
        foreach (var config in _weaponConfigs)
            _weaponList.Add(new Weapon(config));
    }
}
