using Battle_;

namespace GameStateMachineComponents.States
{
    public class MainMenuState : GameState
    {
        public MainMenuState(GameStateMachineData data, IGameStateSwitcher stateSwitcher) : base(data, stateSwitcher)
        {
        }

        public override void Enter()
        {
            BattleSettings.BattleSettingsSaved += OnBattleSettingsSaved;
        }

        public override void Exit()
        {
            BattleSettings.BattleSettingsSaved -= OnBattleSettingsSaved;
        }

        public override void Tick()
        {
        }

        private void OnBattleSettingsSaved()
        {
            StateSwitcher.SwitchState<LevelLoadState>();
        }
    }
}