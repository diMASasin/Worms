using GameStateMachine;
using Pools;
using Timers;

namespace GameBattleStateMachine.States
{
    public class RetreatState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;
        private readonly Timer _timer = new Timer();
        private bool _timerElapsed;
        
        public RetreatState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            ProjectilePool.CountChanged += OnCountChanged;
        }

        public void Exit()
        {
            ProjectilePool.CountChanged -= OnCountChanged;
        }

        public void Tick()
        {
        }

        private void OnCountChanged(int count)
        {
            if(count <= 0)
                _timer.Start(_data.TimersConfig.AfterShotDuration, () => 
                    _stateSwitcher.SwitchState<BetweenTurnsState>());

        }
    }
}