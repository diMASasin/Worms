using System;
using UnityEngine;
using Weapons;

namespace WormComponents
{
    public interface IWormWeapon
    {
        void ChangeWeapon(IWeapon weapon);
        event Action WeaponRemoved;
        Transform WeaponPosition { get; }
    }
}