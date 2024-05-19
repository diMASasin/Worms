using GameStateMachineComponents;
using GameStateMachineComponents.States;
using UI;
using UnityEngine;

namespace Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private LoadingScreen _loadingScreen;
        [SerializeField] private CoroutinePerformer _coroutinePerformer;

        private static GameBootstrapper _instance;
        
        private SceneLoader _sceneLoader;
        private GameStateMachine _gameStateMachine;
        private Game _game;
        private GameStateMachineData _data;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _coroutinePerformer.Init();
            _sceneLoader = new SceneLoader(_coroutinePerformer);
            DontDestroyOnLoad(this);

            _game = new Game();
            _loadingScreen.Init(_sceneLoader);
            
            _data = new GameStateMachineData(_coroutinePerformer, _game, _sceneLoader, _loadingScreen);
            _game.StateMachine = new GameStateMachine(_data);
        }
    }
}