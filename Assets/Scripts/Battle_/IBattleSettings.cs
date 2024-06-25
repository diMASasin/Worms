using System;

namespace Battle_
{
    public interface IBattleSettings
    {
        public SettingsData Data { get; }
        public event Action BattleSettingsSaved;
        public void Save(SettingsData data);
    }
}