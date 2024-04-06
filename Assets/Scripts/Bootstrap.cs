using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Pools;
using ScriptBoy.Digable2DTerrain;
using UnityEngine;

public class Bootstrap : MonoBehaviour, ICoroutinePerformer
{
    [SerializeField] private WeaponConfig[] _weaponConfigs;
    [SerializeField] private WeaponSelector _weaponSelector;
    [SerializeField] private FollowingCamera _followingCamera;
    [SerializeField] private GlobalTimerView _globalTimerView;
    [SerializeField] private Water _water;
    [SerializeField] private Game _game;
    [SerializeField] private TimerView _timerView;
    [SerializeField] private Shovel _shovel;
    [SerializeField] private Wind _wind;
    [SerializeField] private Transform _projectilesParent;

    [SerializeField] private ObjectPoolData<Explosion> _explosionPoolData;
    [SerializeField] private ProjectilePool _fragmentsPool;
    
    private ProjectileData _projectileData;
    private ObjectPool<Explosion> _explosionPool;
    
    private AfterTurnTimer _afterTurnTimer;
    private TurnTimer _turnTimer;
    private Timer _globalTimer;

    private readonly List<Weapon> _weaponList = new();
    
    private void Awake()
    {
        InitializePools();
        CreateWeapon();

        _weaponSelector.Init(_weaponList);
        _followingCamera.Init();
        
        InitializeTimers();

        _water.Init(_globalTimer);
    }

    private void InitializePools()
    {
        _explosionPool = new ObjectPool<Explosion>(_explosionPoolData.Prefab, _projectilesParent, _explosionPoolData.Amount);
        _explosionPool.CreateObjects();

        _projectileData = new ProjectileData(_explosionPool, _shovel, _wind, _fragmentsPool);
        
        _fragmentsPool.Init(_projectilesParent, _projectileData);
        _fragmentsPool.Pool.CreateObjects();

        foreach (var weaponConfig in _weaponConfigs)
        {
            weaponConfig.ProjectilePool.Init(_projectilesParent, _projectileData);
            weaponConfig.ProjectilePool.Pool.CreateObjects();
        }
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
    }

    private void CreateWeapon()
    {
        foreach (var config in _weaponConfigs)
            _weaponList.Add(new Weapon(config));
    }

    public void StartRoutine(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }
}
