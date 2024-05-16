using GameStateMachine;
using Pools;
using Timers;

namespace GameBattleStateMachine.States
{
    public class RetreatState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
        private bool _timerElapsed;

        private Timer Timer => _data.TurnTimer;
        
        public RetreatState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            Timer.Start(_data.TimersConfig.AfterShotDuration, () => 
                _stateSwitcher.SwitchState<ProjectilesWaiting>());
        }

        public void Exit()
        {
            _data.Input.Disable();
            _data.CurrentWorm.SetWormLayer();
        }

        public void Tick()
        {
        }
    }
}