using System;
using Infrastructure;
using Services;
using UI;
using UnityEngine;

namespace GameStateMachineComponents
{
    public class GameStateMachineData
    {
        public readonly Game Game;
        public readonly LoadingScreen LoadingScreen;
        public readonly MainMenu MainMenuPrefab;
        public readonly CoroutinePerformer CoroutinePerformerPrefab;
        public readonly Transform GameParent;

        public GameStateMachineData(Game game, LoadingScreen loadingScreen, MainMenu mainMenuPrefab, 
            CoroutinePerformer coroutinePerformerPrefab, Transform gameParent)
        {
            Game = game;
            LoadingScreen = loadingScreen;
            MainMenuPrefab = mainMenuPrefab;
            CoroutinePerformerPrefab = coroutinePerformerPrefab;
            GameParent = gameParent;
        }
    }
}