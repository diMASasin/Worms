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
        [SerializeField] private TMP_Dropdown _dropdownTeams;
        [SerializeField] private TMP_Dropdown _dropdownWorms;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Image _image;
        [SerializeField] private MapSelectorItemFactory _itemFactory;
        
        private static readonly int ShowAnimation = Animator.StringToHash("Show");
        private static readonly int HideAnimation = Animator.StringToHash("Hide");
        private IBattleSettings _battleSettings;
        private ISceneLoader _sceneLoader;
        private MapSelectorItem _selectedItem;

        [Inject]
        public void Construct(IBattleSettings battleSettings, ISceneLoader sceneLoader)
        {
            _battleSettings = battleSettings;
            _sceneLoader = sceneLoader;
        }

        private void Start()
        {
            _itemFactory.Create();
        }

        private void OnEnable()
        {
            _playButton.onClick.AddListener(Play);
            _closeButton.onClick.AddListener(Hide);
            
            _itemFactory.LevelSelected += OnLevelSelected;
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(Play);
            _closeButton.onClick.RemoveListener(Hide);
            
            _itemFactory.LevelSelected -= OnLevelSelected;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            
        }

        public void Hide() 
        {
            gameObject.SetActive(false);
        }

        public void Close()
        {
        }

        public void Play()
        {
            Hide();
            
            int teamsCount = _dropdownTeams.value + 2;
            int wormsCount = _dropdownWorms.value + 1;
            
            _battleSettings.Save(new SettingsData(wormsCount, teamsCount));
            
            _sceneLoader.Load(_selectedItem.Name);
        }

        private void OnLevelSelected(MapSelectorItem mapSelectorItem)
        {
            _selectedItem = mapSelectorItem;
            
            _image.sprite = _selectedItem.Sprite;
        }
    }
}
