using System;
using System.Collections.Generic;
using EventProviders;
using GameBattleStateMachine;
using GameBattleStateMachine.States;
using PlayerInputSystem;

public class Game : IDisposable
{
    private readonly List<Team> _aliveTeams;
    private readonly BattleStateMachine _battleStateMachine; 
    
    public Game(BattleStateMachineData data)
    {
        _battleStateMachine = new BattleStateMachine(data);
    }
                        
    private void OnWeaponShot(float f)
    {
        _battleStateMachine.SwitchState<RetreatState>();
    }

    public void Dispose()
    {
        _battleStateMachine.Dispose();
    }

    public void Tick() => _battleStateMachine.Tick();
    public void FixedTick() => _battleStateMachine.FixedTick();

    public void Start() => _battleStateMachine.SwitchState<BetweenTurnsState>();
}