using GameStateMachine;
using Timers;

namespace GameBattleStateMachine.States
{
    public class BetweenTurnsState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
        private Timer _timer = new Timer();
        
        public BetweenTurnsState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }
        
        public void Enter()
        {
            _timer.Start(_data.TimersConfig.AfterTurnWaitingDuration, 
                () => _stateSwitcher.SwitchState<TurnState>());
        }

        public void Exit()
        {
        }

        public void Tick()
        {
        }
    }
}