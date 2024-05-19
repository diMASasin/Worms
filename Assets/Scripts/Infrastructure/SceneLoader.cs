using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class SceneLoader
    {
        public const string MainMenu = nameof(MainMenu);
        public const string Map3 = nameof(Map3);
        
        private readonly CoroutinePerformer _coroutinePerformer;

        public event Action<float> ProgressChanged;

        public SceneLoader(CoroutinePerformer coroutinePerformer)
        {
            _coroutinePerformer = coroutinePerformer;
        }
    
        public void Load(string name, Action onLoaded = null) => 
            _coroutinePerformer.StartRoutine(LoadScene(name, onLoaded));

        private IEnumerator LoadScene(string name, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == name)
            {
                onLoaded?.Invoke();
                yield break;
            }
            
            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(name);

            do
            {
                ProgressChanged?.Invoke(waitNextScene.progress);
                yield return null;
            }while (waitNextScene.isDone == false);
        
            onLoaded?.Invoke();
        }
    }
}