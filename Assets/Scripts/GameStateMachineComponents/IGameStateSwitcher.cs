using GameStateMachineComponents.States;

namespace GameStateMachineComponents
{
    public interface IGameStateSwitcher
    {
        public void SwitchState<T>() where T : IGameState;
    }
}