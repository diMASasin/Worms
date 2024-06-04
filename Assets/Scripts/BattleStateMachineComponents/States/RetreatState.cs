using BattleStateMachineComponents.StatesData;
using Configs;
using Timers;

namespace BattleStateMachineComponents.States
{
    public class RetreatState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly GlobalBattleData _data;
        private bool _timerElapsed;

        private TimersConfig TimersConfig => _data.TimersConfig;

        public RetreatState(IStateSwitcher stateSwitcher, GlobalBattleData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            _data.GlobalTimer.Resume();
            _data.CurrentWorm.Input.Enable();
            _data.CurrentWorm.SetCurrentWormLayer();
            _data.CurrentWorm.UnfreezePosition();
            
            _data.TurnTimer.Start(TimersConfig.AfterShotDuration, () => 
                _stateSwitcher.SwitchState<ProjectilesWaiting>());
        }


        public void Exit()
        {
            _data.GlobalTimer.Pause();
            _data.CurrentWorm.Input.Disable();
            _data.CurrentWorm.SetWormLayer();
            _data.CurrentWorm.FreezePositionWhenGrounded();
            
            _data.TurnTimer.Stop();
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