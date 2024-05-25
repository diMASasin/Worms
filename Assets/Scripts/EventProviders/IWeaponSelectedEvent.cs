using System;
using Pools;
using Weapons;

namespace EventProviders
{
    public interface IWeaponSelectedEvent
    {
        public event Action<Weapon, ProjectilePool> WeaponSelected;
    }
}