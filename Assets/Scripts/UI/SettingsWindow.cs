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

        private const string TeamsNumber = nameof(TeamsNumber);
        private const string WormsNumber = nameof(WormsNumber);

        public void Show()
        {
            gameObject.SetActive(true);
            _animator.SetTrigger("Show");
        }

        public void Hide() 
        {
            _animator.SetTrigger("Hide");
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Play()
        {
            Debug.Log("Teams: " + (_dropdownTeams.value + 2));
            Debug.Log("Worms: " + (_dropdownWorms.value + 1));
            PlayerPrefs.SetInt(TeamsNumber, _dropdownTeams.value + 2);
            PlayerPrefs.SetInt(WormsNumber, _dropdownWorms.value + 1);
            SceneManager.LoadScene(_dropdownMaps.value + 1);
        }
    }
}
