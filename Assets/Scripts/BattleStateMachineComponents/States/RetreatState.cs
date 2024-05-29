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

        private Timer Timer => _data.TurnTimer;
        private TimersConfig TimersConfig => _data.TimersConfig;

        public RetreatState(IStateSwitcher stateSwitcher, GlobalBattleData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            _data.GlobalTimer.Resume();
            _data.PlayerInput.MovementInput.Enable(_data.CurrentWorm.Movement);
            
            Timer.Start(TimersConfig.AfterShotDuration, () => 
                _stateSwitcher.SwitchState<ProjectilesWaiting>());
        }


        public void Exit()
        {
            _data.GlobalTimer.Pause();
            _data.PlayerInput.MovementInput.Disable();
            _data.CurrentWorm.Movement.Reset();
            
            Timer.Stop();
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void HandleInput()
        {
            _data.PlayerInput.MovementInput.Tick();
        }
    }
}