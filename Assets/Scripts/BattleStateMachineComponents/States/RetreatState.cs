using Configs;
using Timers;

namespace BattleStateMachineComponents.States
{
    public class RetreatState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
        private bool _timerElapsed;

        private Timer Timer => _data.TurnTimer;
        private TimersConfig TimersConfig => _data.TimersConfig;

        public RetreatState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            Timer.Start(TimersConfig.AfterShotDuration, () => 
                _stateSwitcher.SwitchState<ProjectilesWaiting>());
        }


        public void Exit()
        {
            _data.PlayerInput.MovementInput.Disable();
            Timer.Stop();
        }

        public void Tick()
        {
        }

        public void HandleInput()
        {
            _data.PlayerInput.MovementInput.Tick();
        }
    }
}