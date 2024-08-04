using Battle_;
using UI_;

namespace GameStateMachineComponents.States
{
    public class MainMenuState : GameState
    {
        private readonly IBattleSettings _battleSettings;
        private readonly MainMenu _mainMenu;

        public MainMenuState(IGameStateSwitcher stateSwitcher, IBattleSettings battleSettings, MainMenu mainMenu) : 
            base(stateSwitcher)
        {
            _mainMenu = mainMenu;
            _battleSettings = battleSettings;
        }

        public override void Enter()
        {
            _mainMenu.gameObject.SetActive(true);
            
            _battleSettings.BattleSettingsSaved += OnBattleSettingsSaved;
        }

        public override void Exit()
        {
            _battleSettings.BattleSettingsSaved -= OnBattleSettingsSaved;
            
            _mainMenu.gameObject.SetActive(false);
        }

        private void OnBattleSettingsSaved()
        {
            StateSwitcher.SwitchState<LevelLoadState>();
        }
    }
}