using System;
using UnityEngine;
using Weapons;

namespace WormComponents
{
    public interface IWormWeapon
    {
        Transform WeaponPosition { get; }
        
        event Action<Weapon> WeaponChanged;
        event Action<Weapon> WeaponRemoved;
        
        void ChangeWeapon(Weapon weapon);
    }
}