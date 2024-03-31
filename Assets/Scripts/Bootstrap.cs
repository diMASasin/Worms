using System.Collections;
using System.Collections.Generic;
using Configs;
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
    
    private AfterTurnTimer _afterTurnTimer;
    private TurnTimer _turnTimer;
    private Timer _globalTimer;

    private readonly List<Weapon> _weaponList = new();
    
    private void Awake()
    {
        CreateWeapon();
        _weaponSelector.Init(_weaponList);
        _followingCamera.Init();
        
        _turnTimer = new TurnTimer(_game, _weaponSelector, 60f);
        _timerView.Init(_turnTimer);

        _afterTurnTimer = new AfterTurnTimer(_turnTimer, _game, this, 5);

        _globalTimer = new Timer();
        _globalTimerView.Init(_globalTimer);
        _globalTimer.Start(900);

        _water.Init(_globalTimer);
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
