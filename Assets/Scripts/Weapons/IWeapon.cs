using System;
using Configs;

namespace Weapons
{
    public interface IWeapon
    {
        void MoveScope(float direction);
        void StartIncresePower();
        void IncreaseShotPower();
        void Shoot();
        void Reset();
        float CurrentShotPower { get; }
        public WeaponConfig Config { get; }
        
        public event Action<float> Shot;
        public event Action<float> ShotPowerChanged;
        public event Action IncreasePowerStarted;
        public event Action<float> ScopeMoved;
    }
}