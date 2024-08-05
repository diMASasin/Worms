using Infrastructure.Interfaces;
using UI_;
using Zenject;

namespace GameStateMachineComponents.States
{
    public class LevelLoadState : IGameState
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly LoadingScreen _loadingScreen;
        private IGameStateSwitcher _stateSwitcher;

        public LevelLoadState(DiContainer diContainer, IGameStateSwitcher stateSwitcher, ISceneLoader sceneLoader,
            LoadingScreen loadingScreen)
        {
            _stateSwitcher = stateSwitcher;
            _loadingScreen = loadingScreen;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            _loadingScreen.Enable();
            
            _sceneLoader.Load(_sceneLoader.SceneNames.MainMenu, () => _stateSwitcher.SwitchState<GameLoopState>());
        }

        public void Exit()
        {
        }
    }
}