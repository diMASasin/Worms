using UI;
using Zenject;

namespace GameStateMachineComponents.States
{
    public class GameLoopState : GameState
    {
        private LoadingScreen _loadingScreen;

        public GameLoopState(DiContainer diContainer, IGameStateSwitcher stateSwitcher, LoadingScreen loadingScreen) : base(stateSwitcher)
        {
            _loadingScreen = loadingScreen;
        }
        
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }
    }
}