using Battle_;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class SettingsWindow : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Dropdown _dropdownTeams;
        [SerializeField] private TMP_Dropdown _dropdownWorms;
        [SerializeField] private TMP_Dropdown _dropdownMaps;
        
        private static readonly int ShowAnimation = Animator.StringToHash("Show");
        private static readonly int HideAnimation = Animator.StringToHash("Hide");

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
            
            BattleSettings.Save(wormsCount, teamsCount);
            
            SceneManager.LoadScene(sceneNumber);
        }
    }
}
