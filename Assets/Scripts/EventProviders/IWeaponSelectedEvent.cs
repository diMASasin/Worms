using System;
using Weapons;

namespace EventProviders
{
    public interface IWeaponSelectedEvent
    {
        public event Action<Weapon> WeaponSelected;
    }
}