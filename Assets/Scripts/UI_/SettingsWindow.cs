using Battle_;
using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _UI
{
    public class SettingsWindow : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Dropdown _dropdownTeams;
        [SerializeField] private TMP_Dropdown _dropdownWorms;
        [SerializeField] private TMP_Dropdown _dropdownMaps;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _closeButton;
        
        private static readonly int ShowAnimation = Animator.StringToHash("Show");
        private static readonly int HideAnimation = Animator.StringToHash("Hide");
        private IBattleSettings _battleSettings;
        private ISceneLoader _sceneLoader;

        [Inject]
        public void Construct(IBattleSettings battleSettings, ISceneLoader sceneLoader)
        {
            _battleSettings = battleSettings;
            _sceneLoader = sceneLoader;
        }

        private void OnEnable()
        {
            _playButton.onClick.AddListener(Play);
            _closeButton.onClick.AddListener(Hide);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(Play);
            _closeButton.onClick.RemoveListener(Hide);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _animator.SetTrigger(ShowAnimation);
        }

        public void Hide() 
        {
            _animator.SetTrigger(HideAnimation);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Play()
        {
            int teamsCount = _dropdownTeams.value + 2;
            int wormsCount = _dropdownWorms.value + 1;
            int sceneNumber = _dropdownMaps.value + 1;
            
            _battleSettings.Save(new SettingsData(wormsCount, teamsCount));
            
            _sceneLoader.LoadBattleMap(sceneNumber);
        }
    }
}
