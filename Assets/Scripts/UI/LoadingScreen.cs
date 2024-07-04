using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _percents;
        [SerializeField] private Slider _progressBar;
        
        private ISceneLoader _sceneLoader;

        public void Init(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void OnProgressChanged(float progress)
        {
            if (progress >= 0.89f) progress = 1;
            
            _percents.text = string.Format("{0:F}", $"{progress * 100} %");
            _progressBar.value = progress;
        }

        public void Enable()
        {
            _sceneLoader.ProgressChanged += OnProgressChanged;
            
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }

        public void Disable()
        {
            _sceneLoader.ProgressChanged -= OnProgressChanged;
            
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
        
    }
}