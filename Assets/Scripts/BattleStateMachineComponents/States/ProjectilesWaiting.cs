using Pools;
using Timers;

namespace BattleStateMachineComponents.States
{
    public class ProjectilesWaiting : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;

        public ProjectilesWaiting(IStateSwitcher stateSwitcher)
        {
            _stateSwitcher = stateSwitcher;
        }

        public void Enter()
        {
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

        public void HandleInput()
        {
        }

        private void OnCountChanged(int count)
        {
            if(count == 0)
                _stateSwitcher.SwitchState<BetweenTurnsState>();

        }
    }
}