using System;
using System.Collections;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class SceneNames
    {
        public const string BattleScenePrefix = "Map";
        
        public readonly string MainMenu = nameof(MainMenu);
        public readonly string Map3 = nameof(Map3);
    }

    public class SceneLoader : ISceneLoader
    {
        private readonly ICoroutinePerformer _coroutinePerformer;

        public SceneNames SceneNames { get; } = new();

        public event Action<float> ProgressChanged;

        public SceneLoader(CoroutinePerformer coroutinePerformer)
        {
            _coroutinePerformer = coroutinePerformer;
        }
    
        public void Load(string name, Action onLoaded = null) => 
            _coroutinePerformer.StartCoroutine(LoadScene(name, onLoaded));
        
        public void LoadBattleMap(int index, Action onLoaded = null)
        {
            _coroutinePerformer.StartCoroutine(LoadScene($"{SceneNames.BattleScenePrefix}{index}", onLoaded));
        }

        private IEnumerator LoadScene(string name, Action onLoaded = null)
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == name)
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