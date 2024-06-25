using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Configs;
using Timers;
using UltimateCC;

namespace BattleStateMachineComponents.States
{
    public class RetreatState : IBattleState
    {
        private readonly IBattleStateSwitcher _battleStateSwitcher;
        private readonly BattleStateMachineData _data;
        private bool _timerElapsed;
        private IMovementInput _movementInput;

        private TimersConfig TimersConfig => _data.TimersConfig;

        public RetreatState(IBattleStateSwitcher battleStateSwitcher, BattleStateMachineData data, IMovementInput movementInput)
        {
            _movementInput = movementInput;
            _battleStateSwitcher = battleStateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            _data.BattleTimer.Resume();
            _data.CurrentWorm.DelegateInput(_movementInput);
            _data.WhenMoveCameraFollower.Enable();
            
            _data.TurnTimer.Start(TimersConfig.RetreatDuration, () => 
                _battleStateSwitcher.SwitchState<ProjectilesWaiting>());
        }

        public void Exit()
        {
            _data.BattleTimer.Pause();
            _data.TurnTimer.Stop();
            
            _data.CurrentWorm.RemoveInput();
            _data.WhenMoveCameraFollower.Disable();
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void LateTick()
        {
        }

        public void HandleInput()
        {
            // _data.PlayerInput.MovementInput.Tick();
        }
    }
}