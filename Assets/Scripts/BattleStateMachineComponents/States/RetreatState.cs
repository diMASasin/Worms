using CameraFollow;
using Configs;
using UltimateCC;

namespace BattleStateMachineComponents.States
{
    public class RetreatState : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly BattleStateMachineData _data;
        private bool _timerElapsed;
        private readonly IMovementInput _movementInput;
        private readonly IFollowingCamera _followingCamera;
        private FollowingCameraEventsListener _followingCameraEventsListener;

        private TimersConfig TimersConfig => _data.BattleConfig.TimersConfig;

        public RetreatState(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data, IMovementInput movementInput,
            IFollowingCamera followingCamera, FollowingCameraEventsListener followingCameraEventsListener)
        {
            _followingCameraEventsListener = followingCameraEventsListener;
            _followingCamera = followingCamera;
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