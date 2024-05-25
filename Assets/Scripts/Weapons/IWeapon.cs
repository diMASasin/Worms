using System;
using Configs;

namespace Weapons
{
    public interface IWeapon : IWeaponEventsAndConfig
    {
        void MoveScope(float direction);
        void StartIncresePower();
        void IncreaseShotPower();
        void Shoot();
        void Reset();
        float CurrentShotPower { get; }
    }
}