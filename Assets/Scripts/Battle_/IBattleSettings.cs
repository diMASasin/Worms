using System;
using Services;

namespace Battle_
{
    public interface IBattleSettings : IService
    {
        public SettingsData Data { get; }
        public event Action BattleSettingsSaved;
        public void Save(SettingsData data);
    }
}