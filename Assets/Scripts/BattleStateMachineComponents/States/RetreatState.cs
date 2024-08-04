using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Input_System;
using CameraFollow;
using Configs;

namespace BattleStateMachineComponents.States
{
    public class RetreatState : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly BattleStateMachineData _data;
        private bool _timerElapsed;
        private readonly IMovementInput _movementInput;
        private readonly FollowingCameraEventsListener _followingCameraEventsListener;

        private TimersConfig TimersConfig => _data.BattleConfig.TimersConfig;

        public RetreatState(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data, IMovementInput movementInput,
            FollowingCameraEventsListener followingCameraEventsListener)
        {
            _followingCameraEventsListener = followingCameraEventsListener;
            _movementInput = movementInput;
            _battleStateSwitcher = battleStateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            _data.BattleTimer.Resume();
            _data.CurrentWorm.DelegateInput(_movementInput);
            _followingCameraEventsListener.FollowWormIfMove();
            
            _data.TurnTimer.Start(TimersConfig.RetreatDuration, () => 
                _battleStateSwitcher.SwitchState<ProjectilesWaiting>());
        }

        public void Exit()
        {
            _data.BattleTimer.Pause();
            _data.TurnTimer.Stop();
            _followingCameraEventsListener.StopListenMovement();
            
            _data.CurrentWorm.RemoveInput();
        }
    }
}