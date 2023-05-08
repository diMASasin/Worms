using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterTurnTimer : MonoBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private TurnTimer _turnTimer;
    [SerializeField] private Timer _timer;
    [SerializeField] private Game _game;
    [SerializeField] private WeaponSelector _weaponSelector;

    private void OnEnable()
    {
        foreach (var weapon in _weaponSelector.Weapons)
            weapon.Shot += OnShot;
        _turnTimer.WormShot += OnShot;
    }

    private void OnDisable()
    {
        foreach (var weapon in _weaponSelector.Weapons)
            weapon.Shot -= OnShot;
        _turnTimer.WormShot -= OnShot;
    }

    private void OnShot(Projectile projectile)
    {
        StartTimer();
    }

    private void OnShot()
    {
        StartTimer();
    }

    private void StartTimer()
    {
        _timer.StartTimer(_time, OnTimerOut);
    }

    private void OnTimerOut()
    {
        _timer.StopTimer();
        _game.EndTurn();
        //_game.StartNextTurnWithDelay(1.5f);
        StartCoroutine(_game.WaitUntilProjectilesExplode(() => _game.StartNextTurnWithDelay(1.5f)));
    }
}
