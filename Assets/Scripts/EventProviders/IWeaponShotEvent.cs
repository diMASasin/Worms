using System;
using Weapons;

namespace EventProviders
{
    public interface IWeaponShotEvent
    {
        event Action<float, Weapon> WeaponShot;
    }
}