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
        var currentWorm = _game.GetCurrentTeam().GetCurrentWorm();
        if (currentWorm.Weapon?.CurrentShotPower > 0)
        {
            currentWorm.Weapon.Shoot();
            WormShot?.Invoke();
            return;
        }

        _game.EndTurn();
        _game.StartNextTurnWithDelay(0);
        TimerOut?.Invoke();
    }

    private void OnShot(Projectile projectile)
    {
        _timer.StopTimer();
    }
}
