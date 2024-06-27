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
        private readonly WhenMoveCameraFollower _whenMoveCameraFollower;

        private TimersConfig TimersConfig => _data.TimersConfig;

        public RetreatState(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data, IMovementInput movementInput,
            WhenMoveCameraFollower whenMoveCameraFollower)
        {
            _whenMoveCameraFollower = whenMoveCameraFollower;
            _movementInput = movementInput;
            _battleStateSwitcher = battleStateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            _data.BattleTimer.Resume();
            _data.CurrentWorm.DelegateInput(_movementInput);
            _whenMoveCameraFollower.Enable();
            
            _data.TurnTimer.Start(TimersConfig.RetreatDuration, () => 
                _battleStateSwitcher.SwitchState<ProjectilesWaiting>());
        }

        public void Exit()
        {
            _data.BattleTimer.Pause();
            _data.TurnTimer.Stop();
            
            _data.CurrentWorm.RemoveInput();
            _whenMoveCameraFollower.Disable();
        }
    }
}