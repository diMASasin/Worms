using System.Collections;
using System.Collections.Generic;
using Configs;
using ScriptBoy.Digable2DTerrain;
using UnityEngine;

public class Bootstrap : MonoBehaviour, ICoroutinePerformer
{
    [SerializeField] private WeaponConfig[] _weaponConfigs;
    [SerializeField] private ProjectilePoolAbstract[] _pools;
    [SerializeField] private WeaponSelector _weaponSelector;
    [SerializeField] private FollowingCamera _followingCamera;
    [SerializeField] private GlobalTimerView _globalTimerView;
    [SerializeField] private Water _water;
    [SerializeField] private Game _game;
    [SerializeField] private TimerView _timerView;
    [SerializeField] private Shovel _shovel;
    [SerializeField] private Wind _wind;
    [SerializeField] private Transform _projectilesParent;

    [SerializeField] private Explosion _explosionPrefab;
    [SerializeField] private Projectile _rocketPrefab;
    [SerializeField] private FragmentationGranadeProjectile _fragmentationGranadePrefab;
    [SerializeField] private GranadeProjectile _granadePrefab;
    [SerializeField] private Fragment _fragmentPrefab;
    [SerializeField] private SheepProjectile _sheepPrefab;
    
    private ProjectilesPool _fragmentsPool;
    private ProjectilesPool _rocketPool;
    private ExplosionPool _explosionPool;
    private FragmentationGranadesPool _fragmentationGranadesPool;
    private ProjectilesPool _granadesPool;
    private ProjectilesPool _sheepPool;

    private ProjectilesCounter _projectilesCounter;

    private AfterTurnTimer _afterTurnTimer;
    private TurnTimer _turnTimer;
    private Timer _globalTimer;

    private readonly List<Weapon> _weaponList = new();
    
    private void Awake()
    {
        InitializePools();

        _projectilesCounter = new ProjectilesCounter(_granadesPool, _rocketPool, _sheepPool, _fragmentsPool, _fragmentationGranadesPool);

        _game.Init(_projectilesCounter);

        CreateWeapon();
        _weaponSelector.Init(_weaponList);
        _followingCamera.Init();
        
        InitializeTimers();

        _water.Init(_globalTimer);
    }

    private void InitializePools()
    {
        _explosionPool = new ExplosionPool(_explosionPrefab, _projectilesParent, 10);
        _rocketPool = new ProjectilesPool(_explosionPool, _shovel, _wind, 1, _rocketPrefab, _projectilesParent);
        _granadesPool = new ProjectilesPool(_explosionPool, _shovel, _wind, 1, _granadePrefab, _projectilesParent);
        _sheepPool = new ProjectilesPool(_explosionPool, _shovel, _wind, 1, _sheepPrefab, _projectilesParent);
        _fragmentsPool = new ProjectilesPool(_explosionPool, _shovel, _wind, 10, _fragmentPrefab, _projectilesParent);
        _fragmentationGranadesPool = new FragmentationGranadesPool(_explosionPool, _shovel, _wind, 1,
            _projectilesParent, _fragmentationGranadePrefab, _fragmentsPool);
    }

    private void InitializeTimers()
    {
        _turnTimer = new TurnTimer(_game, _weaponSelector, 60f);
        _timerView.Init(_turnTimer);

        _afterTurnTimer = new AfterTurnTimer(_turnTimer, _game, this, 5);

        _globalTimer = new Timer();
        _globalTimerView.Init(_globalTimer);
        _globalTimer.Start(900);
    }

    private void Update()
    {
        _turnTimer.Tick();
        _globalTimer.Tick();
        _afterTurnTimer.Tick();
    }

    private void OnDestroy()
    {
        _turnTimer.Dispose();
        _afterTurnTimer.Dispose();
        _projectilesCounter.Dispose();
    }

    private void CreateWeapon()
    {
        for (int i = 0; i < _weaponConfigs.Length; i++)
        {
            Weapon newWeapon = new(_weaponConfigs[i], _pools[i]);
            _weaponList.Add(newWeapon);
        }
    }

    public void StartRoutine(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }
}
