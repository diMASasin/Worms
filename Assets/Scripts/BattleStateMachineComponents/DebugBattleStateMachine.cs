using UnityEngine;

namespace BattleStateMachineComponents
{
    public class DebugBattleStateMachine : BattleStateMachine
    {
        public override void SwitchState<T>()
        {
            base.SwitchState<T>();
            Debug.Log($"{CurrentState.GetType().Name}");
        }
    }
}