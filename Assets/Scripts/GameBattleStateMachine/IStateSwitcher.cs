using GameBattleStateMachine.States;

namespace GameBattleStateMachine
{
    public interface IStateSwitcher
    {
        void SwitchState<T>() where T : BattleState;
    }
}