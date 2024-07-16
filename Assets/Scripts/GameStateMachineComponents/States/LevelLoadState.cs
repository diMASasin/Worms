using Infrastructure;
using _UI;
using Zenject;

namespace GameStateMachineComponents.States
{
    public class LevelLoadState : GameState
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly LoadingScreen _loadingScreen;

        public LevelLoadState(DiContainer diContainer, IGameStateSwitcher stateSwitcher, ISceneLoader sceneLoader,
            LoadingScreen loadingScreen) : base(stateSwitcher)
        {
            _loadingScreen = loadingScreen;
            _sceneLoader = sceneLoader;
        }

        public override void Enter()
        {
            _loadingScreen.Enable();
            
            _sceneLoader.Load(_sceneLoader.SceneNames.MainMenu, () => StateSwitcher.SwitchState<GameLoopState>());
        }

        public override void Exit()
        {
        }
    }
}