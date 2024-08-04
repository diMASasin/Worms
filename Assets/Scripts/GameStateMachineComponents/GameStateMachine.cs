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
        private List<GameState> _states;
        private GameState _currentState;
        private readonly StateFactory _stateFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingScreen _loadingScreen;
        private readonly IBattleSettings _battleSettings;
        private readonly DiContainer _container;
        private readonly MainMenu _mainMenu;

        public GameStateMachine(DiContainer container, SceneLoader sceneLoader, LoadingScreen loadingScreen, 
            IBattleSettings battleSettings, MainMenu mainMenu)
        {
            _mainMenu = mainMenu;
            _container = container;
            _battleSettings = battleSettings;
            _loadingScreen = loadingScreen;
            _sceneLoader = sceneLoader;
        }

        public void Init()
        {
            _states = new List<GameState>()
            {
                new BootstrapState(_container, this, _sceneLoader, _loadingScreen, _mainMenu),
                new MainMenuState(this, _battleSettings, _mainMenu),
                new LevelLoadState(_container, this, _sceneLoader, _loadingScreen),
                new GameLoopState(_container, this, _loadingScreen)
            };
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