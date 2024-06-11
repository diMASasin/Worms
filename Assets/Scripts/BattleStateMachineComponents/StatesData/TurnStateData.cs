using System;
using EventProviders;
using Projectiles;
using UI;
using UnityEngine;
using Weapons;
using WormComponents;

namespace BattleStateMachineComponents.StatesData
{
    [Serializable]
    public class TurnStateData
    {
        [field: SerializeField] public WeaponSelector WeaponSelector { get; private set; }

        public IWeaponShotEvent WeaponShotEvent { get; private set; }
        public Arrow Arrow { get; private set; }
        public IProjectileEvents AllProjectileEvents { get; private set; }
        public WeaponChanger WeaponChanger { get; private set; }
        public IWormEvents WormEvents { get; private set; }

        public void Init(Arrow arrow, IProjectileEvents allProjectileEvents, WeaponChanger weaponChanger,
            IWormEvents wormEvents, IWeaponShotEvent weaponShotEvent)
        {
            WeaponShotEvent = weaponShotEvent;
            Arrow = arrow;
            AllProjectileEvents = allProjectileEvents;
            WeaponChanger = weaponChanger;
            WormEvents = wormEvents;
        }
    }
}