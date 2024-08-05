using Battle_;
using UI_;

namespace GameStateMachineComponents.States
{
    public class MainMenuState : IGameState
    {
        private readonly IBattleSettings _battleSettings;
        private readonly MainMenu _mainMenu;
        private readonly IGameStateSwitcher _stateSwitcher;

        public MainMenuState(IGameStateSwitcher stateSwitcher, IBattleSettings battleSettings, MainMenu mainMenu)
        {
            _stateSwitcher = stateSwitcher;
            _mainMenu = mainMenu;
            _battleSettings = battleSettings;
        }

        public void Enter()
        {
            _mainMenu.gameObject.SetActive(true);
            
            _battleSettings.BattleSettingsSaved += OnBattleSettingsSaved;
        }

        public void Exit()
        {
            _battleSettings.BattleSettingsSaved -= OnBattleSettingsSaved;
            
            _mainMenu.gameObject.SetActive(false);
        }

        private void OnBattleSettingsSaved()
        {
            _stateSwitcher.SwitchState<LevelLoadState>();
        }
    }
}