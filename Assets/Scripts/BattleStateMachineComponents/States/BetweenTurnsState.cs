using CameraFollow;
using Configs;
using Timers;

namespace BattleStateMachineComponents.States
{
    public class BetweenTurnsState : BattleState
    {
        private TimersConfig GameConfigTimersConfig => Data.GameConfig.TimersConfig;
        private Timer TurnTimer => Data.TurnTimer;
        private FollowingCamera FollowingCamera => Data.FollowingCamera;

        public BetweenTurnsState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : 
            base(stateSwitcher, data) { }

        public override void Enter()
        {
            TurnTimer.Start(GameConfigTimersConfig.BetweenTurnsDuration, 
                () => StateSwitcher.SwitchState<TurnState>());

            FollowingCamera.ZoomTarget();
            FollowingCamera.SetTarget(Data.FollowingCamera.GeneralViewPosition);
            
            Data.WindMediator.ChangeVelocity();
            Data.Water.IncreaseLevelIfAllowed();
            
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