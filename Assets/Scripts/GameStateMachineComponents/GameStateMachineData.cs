using System;
using Infrastructure;
using UI;

namespace GameStateMachineComponents
{
    public class GameStateMachineData
    {
        public readonly CoroutinePerformer CoroutinePerformer;
        public readonly Game Game;
        public readonly SceneLoader SceneLoader;
        public readonly LoadingScreen LoadingScreen;
        
        public GameStateMachineData(CoroutinePerformer coroutinePerformer, Game game, SceneLoader sceneLoader,
            LoadingScreen loadingScreen)
        {
            CoroutinePerformer = coroutinePerformer;
            Game = game;
            SceneLoader = sceneLoader;
            LoadingScreen = loadingScreen;
        }
    }
}