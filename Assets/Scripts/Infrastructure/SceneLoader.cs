using System;
using Cysharp.Threading.Tasks;
using Infrastructure.Interfaces;
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
        public SceneNames SceneNames { get; } = new();

        public event Action<float> ProgressChanged;

        public void Load(string name, Action onLoaded = null) => 
            LoadScene(name, onLoaded).Forget();
        
        public void LoadBattleMap(int index, Action onLoaded = null) => 
            LoadScene($"{SceneNames.BattleScenePrefix}{index}", onLoaded).Forget();

        private async UniTaskVoid LoadScene(string name, Action onLoaded = null)
        {
            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(name);

            do
            {
                ProgressChanged?.Invoke(waitNextScene.progress);
                await UniTask.Yield();
            }
            while (waitNextScene.isDone == false);
        
            onLoaded?.Invoke();
        }

    }
    
}