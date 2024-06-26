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
        protected IBattleState CurrentState { get; private set; }

        [Inject]
        public void Construct(List<IBattleState> states)
        {
            _states = states;
        }

        public virtual void SwitchState<T>() where T : IBattleState
        {
            IBattleState state = _states.FirstOrDefault(state => state is T);

            CurrentState?.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }
    }
}