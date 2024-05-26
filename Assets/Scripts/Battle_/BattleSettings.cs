using System;
using GameStateMachineComponents;

namespace Battle_
{
    public static class BattleSettings
    {
        private static int _wormsCount; 
        private static int _teamsCount; 
        
        public static event Action BattleSettingsSaved;
        
        public static void Save(int wormsCount, int teamsCount)
        {
            _wormsCount = wormsCount;
            _teamsCount = teamsCount;
            
            BattleSettingsSaved?.Invoke();
        }

        public static void GetSettings(out int wormsCount, out int teamsCount)
        {
            wormsCount = _wormsCount;
            teamsCount = _teamsCount;
        }
    }
}