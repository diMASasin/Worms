using GameStateMachineComponents.States;
using Services;

namespace GameStateMachineComponents
{
    public interface IGameStateSwitcher : IService
    {
        public void SwitchState<T>() where T : GameState;
    }
}