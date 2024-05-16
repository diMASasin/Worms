using GameStateMachine;
using Timers;

namespace GameBattleStateMachine.States
{
    public class BetweenTurnsState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;

        private Timer TurnTimer => _data.TurnTimer;
        private FollowingCamera FollowingCamera => _data.FollowingCamera;

        public BetweenTurnsState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            TurnTimer.Start(_data.TimersConfig.BetweenTurnsDuration, 
                () => _stateSwitcher.SwitchState<TurnState>());

            FollowingCamera.ZoomTarget();
            FollowingCamera.SetTarget(_data.GeneralViewPosition);
            
            _data.Wind.ChangeVelocity();
            _data.WaterMediator.IncreaseLevelIfAllowed();
        }

        public void Exit()
        {
        }

        public void Tick()
        {
        }
    }
}