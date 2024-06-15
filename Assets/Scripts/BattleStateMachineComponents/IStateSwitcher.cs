using BattleStateMachineComponents.States;
using Services;

namespace BattleStateMachineComponents
{
    public interface IStateSwitcher : IService
    {
        void SwitchState<T>() where T : IBattleState;
    }
}