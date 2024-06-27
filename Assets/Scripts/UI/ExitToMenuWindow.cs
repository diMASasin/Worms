using BattleStateMachineComponents;
using GameStateMachineComponents;
using GameStateMachineComponents.States;
using Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class ExitToMenuWindow : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        
        private ISceneLoader _sceneLoader;
        private IGameStateSwitcher _gameStateSwitcher;
        private IBattleStateSwitcher _battleBattleStateSwitcher;
        
        private static readonly int ShowAnimation = Animator.StringToHash("Show");
        private static readonly int HideAnimation = Animator.StringToHash("Hide");

        [Inject]
        private void Construct(ISceneLoader sceneLoader, IGameStateSwitcher gameStateSwitcher, IBattleStateSwitcher battleBattleStateSwitcher)
        {
            _sceneLoader = sceneLoader;
            _gameStateSwitcher = gameStateSwitcher;
            _battleBattleStateSwitcher = battleBattleStateSwitcher;
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
            _battleBattleStateSwitcher.SwitchState<ExitBattleState>();
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
