using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using DefaultNamespace;
using DefaultNamespace.Wind;
using Factories;
using Pools;
using ScriptBoy.Digable2DTerrain;
using UnityEngine;
using UnityEngine.Events;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private WeaponConfig[] _weaponConfigs;
    [SerializeField] private WeaponSelector _weaponSelector;
    [SerializeField] private FollowingCamera _followingCamera;
    [SerializeField] private TimerView _globalTimerView;
    [SerializeField] private TimerView _timerView;
    [SerializeField] private Water _water;
    [SerializeField] private Shovel _shovel;
    [SerializeField] private Wind _wind;
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
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private ExplosionPool _explosionPool;

    private ShovelWrapper _shovelWrapper;
    private ProjectileFactory _projectileFactory;
    
    private Game _game;
    private List<Team> _teams;
    private List<Worm> _worms;

    private readonly List<Team> _currentTeams = new();
    private readonly List<Weapon> _weaponList = new();

    public event UnityAction<List<Team>> WormsSpawned;

    private void Awake()
    {
        _shovelWrapper = new ShovelWrapper(_shovel);
        _teamHealthView.Init(this);
        
        _wind = new Wind(_windData);
        _windEffect.Init(_wind);
        _windView.Init(_wind);

        InitializePools();
        CreateWeapon();
        Spawn();

        _projectileFactory = new ProjectileFactory(_wind, _projectilePrefab, _projectilesParent, _explosionPool);
        
        _game = new Game(_currentTeams, _weaponList, _timersConfig, _timerView, _water, _globalTimerView, _wind);
        
        _weaponSelector.Init(_weaponList, _game, this);

        _followingCamera.Init(_game, _weaponSelector, _currentTeams, _worms);
        
        StartGame();
    }

    private void StartGame()
    {
        WormsSpawned?.Invoke(_teams);
        _game.NextTurn();
    }

    private void Spawn()
    {
        _wormsSpawner.GetEdgesForSpawn();
        _teams = _wormsSpawner.SpawnTeams();
        _worms = _wormsSpawner.WormsList;
        
        _currentTeams.AddRange(_teams);

        foreach (var team in _teams)
            team.Died += OnTeamDied;

        foreach (var worm in _wormsSpawner.WormsList)
           worm.Died += OnWormDied;
    }

    private void Update()
    {
        _game.Tick();
    }

    private void OnDestroy()
    {
        if (_game != null)
            _game.Dispose();

        foreach (var team in _teams)
            team.Died -= OnTeamDied;
    }

    private void OnTeamDied(Team team)
    {
        _currentTeams.Remove(team);

        if (_currentTeams.Count <= 1)
            _endScreen.gameObject.SetActive(true);
    }

    private void OnWormDied(Worm worm)
    {
        worm.Died -= OnWormDied;

        if (worm.Input.IsEnabled == true)
            _game.NextTurn(_turnDelay);
    }

    private void InitializePools()
    {
        _fragmentsPool.Init(_projectileFactory);
        _explosionPool.Init(_projectilesParent, _shovelWrapper);

        foreach (var weaponConfig in _weaponConfigs)
            weaponConfig.ProjectilePool.Init(_projectileFactory);
    }

    private void CreateWeapon()
    {
        foreach (var config in _weaponConfigs)
            _weaponList.Add(new Weapon(config));
    }
}
