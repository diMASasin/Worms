using System;
using UnityEngine;

namespace EventProviders
{
    public interface IWeaponShotEvent
    {
        event Action<float> WeaponShot;
    }
}