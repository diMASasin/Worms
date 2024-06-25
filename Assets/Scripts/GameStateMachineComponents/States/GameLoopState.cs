using Zenject;

namespace GameStateMachineComponents.States
{
    public class GameLoopState : GameState
    {
        public GameLoopState(DiContainer diContainer, IGameStateSwitcher stateSwitcher) : base(stateSwitcher)
        {
            
        }
        
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }
    }
}