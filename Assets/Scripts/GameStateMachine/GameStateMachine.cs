using System;
using System.Collections.Generic;
using System.Linq;
using GameStateMachine.States;

namespace GameStateMachine
{
    public class GameStateMachine
    {
        private List<IGameState> _states;
        private IGameState _currentState;

        public GameStateMachine(Game game)
        {

            _states = new List<IGameState>()
            {
                new BootstrapState()
            };

            _currentState = _states[0];
            _currentState.Enter();
        }

        public void SwitchState<T>() where T : IGameState
        {
            IGameState state = _states.FirstOrDefault(state => state is T);

            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        // public void HandleInput() => _currentState.HandleInput();

        public void Update() => _currentState.Tick();
    }
}