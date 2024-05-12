using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Projectiles;
using Timers;

public class TimerMediator
{
    private readonly Timer _globalTimer;
    private readonly Timer _turnTimer;
    private readonly TimersConfig _config;
    private readonly Game _game;
    private readonly IWeaponShotEventProvider _weaponShotEvent;

    public event Action GlobalTimerElapsed;

    public TimerMediator(Timer globalTimer, Timer turnTimer, TimersConfig config, Game game,
        IWeaponShotEventProvider weaponShotEvent)
    {
        _globalTimer = globalTimer;
        _turnTimer = turnTimer;
        _config = config;
        _game = game;
        _weaponShotEvent = weaponShotEvent;

        _game.GameStarted += OnGameStarted;
        _game.TurnStarted += OnTurnStarted;
        _weaponShotEvent.WeaponShot += OnShot;
    }

    public void Dispose()
    {
        _game.GameStarted -= OnGameStarted;
        _game.TurnStarted -= OnTurnStarted;
        _weaponShotEvent.WeaponShot -= OnShot;
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

    private void OnShot(Weapon weapon1)
    {
        _turnTimer.Start(_config.AfterShotDuration, () => _game.StartNextTurn(_config.AfterTurnWaitingDuration));
    }
}