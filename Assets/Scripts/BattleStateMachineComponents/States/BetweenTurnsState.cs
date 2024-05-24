using CameraFollow;
using Timers;

namespace BattleStateMachineComponents.States
{
    public class BetweenTurnsState : BattleState
    {
        private Timer TurnTimer => Data.TurnTimer;
        private FollowingCamera FollowingCamera => Data.FollowingCamera;

        public BetweenTurnsState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : 
            base(stateSwitcher, data) { }

        public override void Enter()
        {
            TurnTimer.Start(Data.TimersConfig.BetweenTurnsDuration, 
                () => StateSwitcher.SwitchState<TurnState>());

            FollowingCamera.ZoomTarget();
            FollowingCamera.SetTarget(Data.FollowingCamera.GeneralViewPosition);
            
            Data.Wind.ChangeVelocity();
            Data.WaterMediator.IncreaseLevelIfAllowed();
            
            if(Data.CurrentWorm != null)
                Data.CurrentWorm.SetWormLayer();
        }

        public override void Exit()
        {
            TurnTimer.Stop();
        }

        public override void Tick()
        {
        }

        public override void HandleInput()
        {
        }
    }
}