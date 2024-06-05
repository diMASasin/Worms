namespace GameStateMachineComponents.States
{
    public abstract class GameState
    {
        public readonly GameStateMachineData Data;
        public readonly IGameStateSwitcher StateSwitcher;

        public GameState(GameStateMachineData data, IGameStateSwitcher stateSwitcher)
        {
            Data = data;
            StateSwitcher = stateSwitcher;
        }

        public abstract void Enter();

        public abstract void Exit();
    }
}