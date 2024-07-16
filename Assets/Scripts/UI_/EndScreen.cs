using Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace _UI
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private Button _button;
        private ISceneLoader _sceneLoader;

        [Inject]
        private void Construct(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        private void OnEnable() => _button.onClick.AddListener(Restart);

        private void OnDisable() => _button.onClick.RemoveListener(Restart);

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        public void Restart() => _sceneLoader.LoadBattleMap(SceneManager.GetActiveScene().buildIndex);
    }
}
