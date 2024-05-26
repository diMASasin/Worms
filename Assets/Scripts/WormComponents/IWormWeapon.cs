using System;
using UnityEngine;
using Weapons;

namespace WormComponents
{
    public interface IWormWeapon
    {
        Transform WeaponPosition { get; }
        
        event Action<IWeapon> WeaponChanged;
        event Action WeaponRemoved;
        
        void ChangeWeapon(IWeapon weapon);
    }
}