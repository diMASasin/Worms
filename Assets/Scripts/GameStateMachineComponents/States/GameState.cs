namespace GameStateMachineComponents.States
{
    public abstract class GameState
    {
        public IGameStateSwitcher StateSwitcher;

        public GameState(IGameStateSwitcher stateSwitcher)
        {
            StateSwitcher = stateSwitcher;
        }

        public abstract void Enter();

        public abstract void Exit();
    }
}