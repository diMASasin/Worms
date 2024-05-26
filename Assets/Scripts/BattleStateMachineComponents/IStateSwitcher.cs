using BattleStateMachineComponents.States;

namespace BattleStateMachineComponents
{
    public interface IStateSwitcher
    {
        void SwitchState<T>() where T : IBattleState;
    }
}