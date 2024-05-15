using Configs;
using GameStateMachine;

namespace GameBattleStateMachine.States
{
    public class StartBattle : IBattleState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly BattleStateMachineData _data;

        private TimersConfig TimersConfig => _data.TimersConfig;
        private WaterMediator WaterMediator => _data.WaterMediator;

        public StartBattle(IStateSwitcher stateSwitcher, BattleStateMachineData data)
        {
            _stateSwitcher = stateSwitcher;
            _data = data;
        }

        public void Enter()
        {
            _data.GlobalTimer.Start(TimersConfig.GlobalTime, () => WaterMediator.AllowIncreaseWaterLevel());
        }

        public void Exit()
        {
        }

        public void Tick()
        {
        }
    }
}