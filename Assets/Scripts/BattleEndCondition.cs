using System;
using BattleStateMachineComponents;
using BattleStateMachineComponents.States;
using BattleStateMachineComponents.StatesData;
using EventProviders;
using WormComponents;

public class BattleEndCondition : IDisposable
{
    private readonly IAliveTeams _aliveTeams;
    private readonly IBattleStateSwitcher _battleStateSwitcher;
    private readonly ITeamDiedEventProvider _teamDiedEvent;

    public BattleEndCondition(IAliveTeams aliveTeams, IBattleStateSwitcher battleStateSwitcher,
        ITeamDiedEventProvider teamDiedEvent)
    {
        _teamDiedEvent = teamDiedEvent;
        _battleStateSwitcher = battleStateSwitcher;
        _aliveTeams = aliveTeams;
        
        _teamDiedEvent.TeamDied += OnTeamDied;
    }

    public void Dispose()
    {
        _teamDiedEvent.TeamDied -= OnTeamDied;
    }

    private void OnTeamDied(Team team)
    {
        _aliveTeams.AliveTeams.Remove(team);
        
        if (_aliveTeams.AliveTeams.Count <= 1) 
            _battleStateSwitcher.SwitchState<BattleEndState>();
    }
}