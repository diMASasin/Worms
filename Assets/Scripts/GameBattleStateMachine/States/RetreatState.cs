using GameStateMachine;
using Pools;
using Timers;

namespace GameBattleStateMachine.States
{
    public class RetreatState : BattleState
    {
        private bool _timerElapsed;

        private Timer Timer => Data.TurnTimer;

        public RetreatState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : base(stateSwitcher, data) { }

        public override void Enter()
        {
            Timer.Start(Data.TimersConfig.AfterShotDuration, () => 
                StateSwitcher.SwitchState<ProjectilesWaiting>());
        }

        public override void Exit()
        {
            Data.PlayerInput.Disable();
        }

        public override void Tick()
        {
        }

        public override void HandleInput()
        {
        }
    }
}