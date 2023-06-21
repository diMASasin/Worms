using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTimer : MonoBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private Game _game;
    [SerializeField] private Timer _timer;
    [SerializeField] private WeaponSelector _weaponSelector;

    public event Action TimerOut;
    public event Action WormShot;
    public event Action TimerStopped;

    private void OnValidate()
    {
        _game = FindObjectOfType<Game>();
    }

    private void OnEnable()
    {
        _game.WormsSpawned += OnWormsSpawned;

        foreach (var weapon in _weaponSelector.Weapons)
            weapon.Shot += OnShot;
    }

    private void OnDisable()
    {
        _game.WormsSpawned -= OnWormsSpawned;

        foreach (var weapon in _weaponSelector.Weapons)
            weapon.Shot -= OnShot;
    }

    private void OnWormsSpawned(List<Team> teams)
    {
        foreach(var team in teams)
        {
            team.TurnStarted += OnTurnStarted;
            team.Died += OnDied;
        }
    }

    private void OnDied(Team team)
    {
        team.TurnStarted -= OnTurnStarted;
        team.Died -= OnDied;
    }

    private void OnTurnStarted(Worm worm, Team team)
    {
        _timer.StartTimer(_time, OnTimerOut);
    }

    private void OnTimerOut()
    {
        var currentWorm = _game.TryGetCurrentTeam().TryGetCurrentWorm();
        if (currentWorm.Weapon?.CurrentShotPower > 0)
        {
            currentWorm.Weapon.Shoot();
            WormShot?.Invoke();
            return;
        }

        _game.DisableCurrentWorm();
        _game.StartNextTurnWithDelay(1);
        TimerOut?.Invoke();
    }

    private void OnShot(Projectile projectile)
    {
        _timer.StopTimer();
        TimerStopped?.Invoke();
    }
}
