using Configs;

namespace BattleStateMachineComponents.States
{
    public class BootstrapState : BattleState
    {
        private TimersConfig TimersConfig => Data.TimersConfig;
        private WaterMediator WaterMediator => Data.WaterMediator;

        public BootstrapState(IStateSwitcher stateSwitcher, BattleStateMachineData data) : base(stateSwitcher, data) { }

        public override void Enter()
        {
            
            
            Data.GlobalTimer.Start(TimersConfig.GlobalTime, () => WaterMediator.AllowIncreaseWaterLevel());
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