using Infrastructure;
using Services;

namespace GameStateMachineComponents.States
{
    public class LevelLoadState : GameState
    {
        private readonly ISceneLoader _sceneLoader;

        public LevelLoadState(GameStateMachineData data, IGameStateSwitcher stateSwitcher, ISceneLoader sceneLoader) :
            base(data, stateSwitcher)
        {
            _sceneLoader = sceneLoader;
        }

        public override void Enter()
        {
            Data.LoadingScreen.Enable();
            
            _sceneLoader.Load(_sceneLoader.SceneNames.MainMenu, () => StateSwitcher.SwitchState<GameLoopState>());
        }

        public override void Exit()
        {
            Data.LoadingScreen.Disable();
        }
    }
}