using System;
using Services;

namespace Infrastructure
{
    public interface ISceneLoader : IService
    {
        event Action<float> ProgressChanged;
        void Load(string name, Action onLoaded = null);
        void LoadBattleMap(int index, Action onLoaded = null);
        public SceneNames SceneNames { get; }
    }
}