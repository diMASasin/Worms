using System.Collections.Generic;
using System.Linq;
using Battle_;
using BattleStateMachineComponents.States;
using Spawn;
using UnityEngine;
using Zenject;

namespace BattleStateMachineComponents
{
    public class BattleStateMachine : IBattleStateSwitcher
    {
        private List<IBattleState> _states;
        private IBattleState _currentState;

        [Inject]
        public void Construct(List<IBattleState> states)
        {
            _states = states;
        }

        public void SwitchState<T>() where T : IBattleState
        {
            IBattleState state = _states.FirstOrDefault(state => state is T);

            _currentState?.Exit();
            _currentState = state;
            Debug.Log($"{_currentState.GetType().FullName}");
            _currentState.Enter();
        }
    }
}