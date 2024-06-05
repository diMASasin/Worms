using System;
using GameStateMachineComponents;

namespace Battle_
{
    public class BattleSettings : IBattleSettings
    {
        private int _wormsCount;
        private int _teamsCount;
        
        public SettingsData Data { get; private set; }

        public event Action BattleSettingsSaved;

        public void Save(SettingsData data)
        {
            Data = data;

            BattleSettingsSaved?.Invoke();
        }
    }

    public class SettingsData
    {
        public readonly int WormsCount;
        public readonly int TeamsCount;

        public SettingsData(int wormsCount, int teamsCount)
        {
            WormsCount = wormsCount;
            TeamsCount = teamsCount;
        }
    }
}