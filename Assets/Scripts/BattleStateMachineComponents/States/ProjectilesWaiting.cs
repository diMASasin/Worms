using Pools;
using Timers;

namespace BattleStateMachineComponents.States
{
    public class ProjectilesWaiting : BattleState
    {
        public ProjectilesWaiting(IStateSwitcher stateSwitcher, BattleStateMachineData data) : base(stateSwitcher, data)
        { }

        public override void Enter()
        {
            OnCountChanged(ProjectilePool.Count);
            
            ProjectilePool.CountChanged += OnCountChanged;
        }

        public override void Exit()
        {
            ProjectilePool.CountChanged -= OnCountChanged;
        }

        public override void Tick()
        {
        }

        public override void HandleInput()
        {
        }

        private void OnCountChanged(int count)
        {
            if(count == 0)
                StateSwitcher.SwitchState<BetweenTurnsState>();

        }
    }
}