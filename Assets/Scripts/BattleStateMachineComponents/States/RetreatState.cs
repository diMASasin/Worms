using Configs;
using Timers;

namespace BattleStateMachineComponents.States
{
    public class RetreatState : BattleState
    {
        private bool _timerElapsed;

        private Timer Timer => Data.TurnTimer;
        private TimersConfig GameConfigTimersConfig => Data.GameConfig.TimersConfig;

        public RetreatState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : base(stateSwitcher, data) { }

        public override void Enter()
        {
            Timer.Start(GameConfigTimersConfig.AfterShotDuration, () => 
                StateSwitcher.SwitchState<ProjectilesWaiting>());
        }


        public override void Exit()
        {
            Data.PlayerInput.MovementInput.Disable();
            Timer.Stop();
        }

        public override void Tick()
        {
        }

        public override void HandleInput()
        {
            Data.PlayerInput.MovementInput.Tick();
        }
    }
}