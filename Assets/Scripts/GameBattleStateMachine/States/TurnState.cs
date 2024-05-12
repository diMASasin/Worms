using System.Collections.Generic;
using Timers;
using GameStateMachine;

namespace GameBattleStateMachine.States
{
    public class TurnState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
        private readonly Timer _timer = new Timer();

        private int CurrentTeamIndex => _data.CurrentTeamIndex;
        private List<Team> AliveTeams => _data.AliveTeams;
        private Worm CurrentWorm => _data.CurrentWorm;
        
        public TurnState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }
        
        public void Enter()
        {
            _data.TryGetNextTeam(out Team team);
            team.TryGetNextWorm(out Worm worm);
            worm.OnTurnStarted();
            
            _data.CurrentTeam = team;
            _data.CurrentWorm = worm;
            
            _timer.Start(_data.TimersConfig.TurnDuration, OnTimerElapsed);
        }

        public void Exit()
        {
        }

        public void Tick()
        {
        }

        private void OnTimerElapsed()
        {
            CurrentWorm.OnTurnEnd();
            
            if (CurrentWorm.Weapon?.CurrentShotPower > 0)
            {
                CurrentWorm.Weapon.Shoot();
                _stateSwitcher.SwitchState<BetweenTurnsState>();
                return;
            }
            _stateSwitcher.SwitchState<RetreatState>();
        }
    }
}