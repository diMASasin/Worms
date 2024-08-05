using System.Collections.Generic;
using System.Linq;
using Battle_;
using GameStateMachineComponents.States;
using Infrastructure;
using UI_;
using Zenject;

namespace GameStateMachineComponents
{
    public class GameStateMachine : IGameStateSwitcher
    {
        private readonly StateFactory _stateFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingScreen _loadingScreen;
        private readonly IBattleSettings _battleSettings;
        private readonly DiContainer _container;
        private readonly MainMenu _mainMenu;
        
        private List<IGameState> _states;
        private BattleBootstraper _battleBootstraper;
        private IGameState _currentState;

        [Inject]
        public void Construct(List<IGameState> states)
        {
            _states = states;
        }

        public void SwitchState<T>() where T : IGameState
        {
            IGameState state = _states.FirstOrDefault(state => state is T);

            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }
    }
}