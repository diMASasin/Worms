using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using GameBattleStateMachine;
using GameBattleStateMachine.States;
using GameStateMachine;
using Projectiles;
using UnityEngine;
using Input = PlayerInput.Input;

public class Game : IDisposable
{
    private readonly List<Team> _aliveTeams;
    private readonly ITeamDiedEventProvider _teamDiedEvent;
    private readonly EndScreen _endScreen;
    private readonly IWeaponShotEvent _weaponShotEvent;
    private readonly BattleStateMachine _battleStateMachine; 
    
    public Game(List<Team> aliveTeams, ITeamDiedEventProvider teamDiedEvent, EndScreen endScreen,
        BattleStateMachineData data, IWeaponShotEvent weaponShotEvent)
    {
        _aliveTeams = aliveTeams;
        
        _teamDiedEvent = teamDiedEvent;
        _endScreen = endScreen;
        _weaponShotEvent = weaponShotEvent;

        _battleStateMachine = new BattleStateMachine(data);
        
        _teamDiedEvent.TeamDied += OnTeamDied;
        _weaponShotEvent.WeaponShot += OnProjectileExploded;
    }

    private void OnProjectileExploded(float f)
    {
        _battleStateMachine.SwitchState<RetreatState>();
    }

    public void Dispose()
    {
        if (_teamDiedEvent != null) _teamDiedEvent.TeamDied -= OnTeamDied;
        if (_weaponShotEvent != null) _weaponShotEvent.WeaponShot -= OnProjectileExploded;
    }

    public void Tick() => _battleStateMachine.Tick();

    public void StartGame() => _battleStateMachine.SwitchState<BetweenTurnsState>();

    private void OnTeamDied(Team team)
    {
        _aliveTeams.Remove(team);
        
        if (_aliveTeams.Count <= 1) 
            _endScreen.Show();
    }
}