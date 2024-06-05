using Battle_;
using Infrastructure;
using Services;
using UI;
using static UnityEngine.Object;

namespace GameStateMachineComponents.States
{
    public class MainMenuState : GameState
    {
        private readonly IBattleSettings _battleSettings;
        private readonly ISceneLoader _sceneLoader;
        private MainMenu _mainMenu;

        public MainMenuState(GameStateMachineData data, IGameStateSwitcher stateSwitcher, 
            IBattleSettings battleSettings, ISceneLoader sceneLoader) : base(data, stateSwitcher)
        {
            _battleSettings = battleSettings;
            _sceneLoader = sceneLoader;
        }

        public override void Enter()
        {
            _mainMenu = Instantiate(Data.MainMenuPrefab);
            _mainMenu.SettingsWindow.Init(_battleSettings, _sceneLoader);

            _battleSettings.BattleSettingsSaved += OnBattleSettingsSaved;
        }

        public override void Exit()
        {
            _battleSettings.BattleSettingsSaved -= OnBattleSettingsSaved;
        }

        private void OnBattleSettingsSaved()
        {
            StateSwitcher.SwitchState<LevelLoadState>();
        }
    }
}