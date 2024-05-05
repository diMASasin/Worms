using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using Timers;

public class TimerMediator
{
    private readonly Timer _globalTimer;
    private readonly Timer _turnTimer;
    private readonly TimersConfig _config;
    private readonly Game _game;
    private List<Weapon> _weaponsList;

    public event Action GlobalTimerElapsed;

    public TimerMediator(Timer globalTimer, Timer turnTimer, TimersConfig config, Game game, List<Weapon> weaponsList)
    {
        _globalTimer = globalTimer;
        _turnTimer = turnTimer;
        _config = config;
        _game = game;
        _weaponsList = weaponsList;

        _game.GameStarted += OnGameStarted;
        _game.TurnStarted += OnTurnStarted;

        foreach (var weapon in _weaponsList)
            weapon.Shot += OnShot;
    }

    public void Dispose()
    {
        _game.GameStarted -= OnGameStarted;
        _game.TurnStarted -= OnTurnStarted;

        foreach (var weapon in _weaponsList)
            weapon.Shot -= OnShot;
    }

    private void OnGameStarted()
    {
        _globalTimer.Start(_config.GlobalTime, OnGlobalTimerElapsed);
    }

    private void OnGlobalTimerElapsed()
    {
        GlobalTimerElapsed?.Invoke();
    }

    private void OnTurnStarted(Worm worm, Team team)
    {
        _turnTimer.Start(_config.TurnDuration, () => _game.StartNextTurn(_config.AfterTurnWaitingDuration));
    }

    private void OnShot(Projectile projectile)
    {
        _turnTimer.Start(_config.AfterShotDuration, () => _game.StartNextTurn(_config.AfterTurnWaitingDuration));
    }
}