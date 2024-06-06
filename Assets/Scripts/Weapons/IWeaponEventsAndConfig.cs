using System;
using Configs;

namespace Weapons
{
    public interface IWeaponEventsAndConfig
    {
        public WeaponConfig Config { get; }
        
        event Action<float, Weapon> Shot;
        event Action<float> ShotPowerChanged;
        event Action IncreasePowerStarted;
        event Action<float> ScopeMoved;
    }
}