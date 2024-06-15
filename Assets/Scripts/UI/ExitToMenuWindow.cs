using BattleStateMachineComponents;
using BattleStateMachineComponents.States;
using GameStateMachineComponents;
using GameStateMachineComponents.States;
using Infrastructure;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class ExitToMenuWindow : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        
        private ISceneLoader _sceneLoader;
        private IGameStateSwitcher _gameStateSwitcher;
        private IStateSwitcher _battleStateSwitcher;
        
        private static readonly int ShowAnimation = Animator.StringToHash("Show");
        private static readonly int HideAnimation = Animator.StringToHash("Hide");

        private void Start()
        {
            _sceneLoader = AllServices.Container.Single<ISceneLoader>();
            _gameStateSwitcher = AllServices.Container.Single<IGameStateSwitcher>();
            _battleStateSwitcher = AllServices.Container.Single<IStateSwitcher>();
        }

        private void OnEnable()
        {
            _yesButton.onClick.AddListener(ExitToMenu);
            _noButton.onClick.AddListener(Hide);
        }

        private void OnDisable()
        {
            _yesButton.onClick.RemoveListener(ExitToMenu);
            _noButton.onClick.RemoveListener(Hide);
        }

        public void ExitToMenu()
        {
            _battleStateSwitcher.SwitchState<ExitBattleState>();
            _sceneLoader.Load(_sceneLoader.SceneNames.MainMenu, () =>
                    _gameStateSwitcher.SwitchState<MainMenuState>());
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _animator.SetTrigger(ShowAnimation);
        }

        private void Hide() 
        { 
            _animator.SetTrigger(HideAnimation);
            gameObject.SetActive(false);
        }
    }
}
