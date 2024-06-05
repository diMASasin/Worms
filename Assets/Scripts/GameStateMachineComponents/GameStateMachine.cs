using System.Collections.Generic;
using System.Linq;
using Battle_;
using BattleStateMachineComponents;
using GameStateMachineComponents.States;
using Infrastructure;
using Services;

namespace GameStateMachineComponents
{
    public class GameStateMachine : IGameStateSwitcher
    {
        private readonly GameStateMachineData _data;
        private readonly List<GameState> _states;
        private GameState _currentState;

        public GameStateMachine(GameStateMachineData data, AllServices services)
        {
            _data = data;

            _states = new List<GameState>()
            {
                new BootstrapState(_data, this, services),
                new MainMenuState(_data, this, services.Single<IBattleSettings>(), services.Single<ISceneLoader>()),
                new LevelLoadState(_data, this, services.Single<ISceneLoader>()),
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
    }
}