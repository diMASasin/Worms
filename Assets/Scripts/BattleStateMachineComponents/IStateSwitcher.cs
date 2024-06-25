using BattleStateMachineComponents.States;

namespace BattleStateMachineComponents
{
    public interface IBattleStateSwitcher
    {
        void SwitchState<T>() where T : IBattleState;
    }
}