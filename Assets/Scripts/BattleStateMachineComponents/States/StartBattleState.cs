using Configs;

namespace BattleStateMachineComponents.States
{
    public class StartBattleState : BattleState
    {
        private TimersConfig TimersConfig => Data.GameConfig.TimersConfig;

        public StartBattleState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : base(stateSwitcher, data) { }

        public override void Enter()
        {
            Data.GlobalTimer.Start(TimersConfig.GlobalTime, () => Data.Water.AllowIncreaseWaterLevel());
            StateSwitcher.SwitchState<BetweenTurnsState>();
        }

        public override void Exit()
        {
        }

        public override void Tick()
        {
        }

        public override void HandleInput()
        {
        }
    }
}