using System;

namespace EventProviders
{
    public interface IWeaponShotEventProvider
    {
        event Action<Weapon> WeaponShot;
    }
}