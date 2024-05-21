using System.Collections.Generic;
using System.Linq;
using BattleStateMachineComponents;
using GameStateMachineComponents.States;

namespace GameStateMachineComponents
{
    public class GameStateMachine : IGameStateSwitcher
    {
        private readonly GameStateMachineData _data;
        private readonly List<GameState> _states;
        private GameState _currentState;

        public GameStateMachine(GameStateMachineData data)
        {
            _data = data;

            _states = new List<GameState>()
            {
                new BootstrapState(_data, this),
                new MainMenuState(_data, this),
                new LevelLoadState(_data, this),
                new GameLoopState(_data, this)
            };

            _currentState = _states[0];
            _currentState.Enter();
        }

        public void SwitchState<T>() where T : GameState
        {
            GameState state = _states.FirstOrDefault(state => state is T);

            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        // public void HandleInput() => _currentState.HandleInput();

        public void Update() => _currentState.Tick();
    }
}