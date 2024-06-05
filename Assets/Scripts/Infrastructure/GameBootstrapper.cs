using Battle_;
using GameStateMachineComponents;
using GameStateMachineComponents.States;
using Services;
using UI;
using UnityEngine;

namespace Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private LoadingScreen _loadingScreen;
        [SerializeField] private CoroutinePerformer _coroutinePerformerPrefab;
        [SerializeField] private MainMenu _mainMenuPrefab;
        
        private static GameBootstrapper _instance;
        
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
            
            DontDestroyOnLoad(this);

            _game = new Game();
            
            _data = new GameStateMachineData(_game, _loadingScreen, _mainMenuPrefab, _coroutinePerformerPrefab, transform);
            _game.StateMachine = new GameStateMachine(_data, AllServices.Container);
        }

        
    }
}