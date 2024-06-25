using Infrastructure;
using Spawn;
using UI;
using Zenject;

namespace GameStateMachineComponents.States
{
    public class LevelLoadState : GameState
    {
        private ISceneLoader _sceneLoader;
        private LoadingScreen _loadingScreen;

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
            _loadingScreen.Disable();
        }
    }
}