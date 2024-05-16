using GameStateMachine;
using Pools;

namespace GameBattleStateMachine.States
{
    public class ProjectilesWaiting : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;

        public ProjectilesWaiting(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            _data.CurrentWorm.SetWormLayer();
            OnCountChanged(ProjectilePool.Count);
            
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
            if(count == 0)
                _stateSwitcher.SwitchState<BetweenTurnsState>();

        }
    }
}