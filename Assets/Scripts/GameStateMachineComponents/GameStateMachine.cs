using System;
using System.Collections.Generic;
using System.Linq;
using Battle_;
using BattleStateMachineComponents;
using GameStateMachineComponents.States;
using Infrastructure;
using Services;

namespace GameStateMachineComponents
{
    public class GameStateMachine : IGameStateSwitcher, IDisposable
    {
        private readonly GameStateMachineData _data;
        private readonly List<GameState> _states;
        private GameState _currentState;
        private readonly BootstrapState _bootstrapState;

        public GameStateMachine(GameStateMachineData data, AllServices services)
        {
            _data = data;
            _bootstrapState = new BootstrapState(_data, this, services);
            
            _states = new List<GameState>()
            {
                _bootstrapState,
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

        public void Dispose()
        {
            _bootstrapState.Dispose();
        }
    }
}