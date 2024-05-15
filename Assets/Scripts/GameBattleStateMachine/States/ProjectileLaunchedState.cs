using GameBattleStateMachine.States;
using Projectiles;

namespace GameBattleStateMachine
{
    public class ProjectileLaunchedState : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;

        public ProjectileLaunchedState(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            _data.ProjectileLauncher.ProjectileExploded += OnProjectileExploded;
        }

        public void Exit()
        {
            _data.ProjectileLauncher.ProjectileExploded -= OnProjectileExploded;
        }

        public void Tick()
        {
        }

        private void OnProjectileExploded(Projectile projectile)
        {
            
        }
    }
}