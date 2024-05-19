using Infrastructure;

namespace GameStateMachineComponents.States
{
    public class LevelLoadState : GameState
    {
        public LevelLoadState(GameStateMachineData data, IGameStateSwitcher stateSwitcher) : 
            base(data, stateSwitcher) { }

        public override void Enter()
        {
            Data.LoadingScreen.Enable();
            Data.SceneLoader.Load(SceneLoader.Map3, () => StateSwitcher.SwitchState<GameLoopState>());
        }

        public override void Exit()
        {
            Data.LoadingScreen.Disable();
        }

        public override void Tick()
        {
        }
    }
}