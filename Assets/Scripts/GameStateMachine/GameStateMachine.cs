using System;
using System.Collections.Generic;
using System.Linq;
using GameStateMachine.States;

namespace GameStateMachine
{
    public class GameStateMachine
    {
        private List<IState> _states;
        private IState _currentState;

        public GameStateMachine(Game game)
        {

            _states = new List<IState>()
            {
                new BootstrapState()
            };

            _currentState = _states[0];
            _currentState.Enter();
        }

        public void SwitchState<T>() where T : IState
        {
            IState state = _states.FirstOrDefault(state => state is T);

            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        // public void HandleInput() => _currentState.HandleInput();

        public void Update() => _currentState.Tick();
    }
}