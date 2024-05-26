using System;
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
        
        public CycledList<Team> AliveTeams { get; private set; } = new();
        public Arrow Arrow { get; private set; }
        public IProjectileEvents AllProjectileEvents { get; private set; }
        public WeaponChanger WeaponChanger { get; private set; }

        public void Init(Arrow arrow, IProjectileEvents allProjectileEvents, WeaponChanger weaponChanger)
        {
            Arrow = arrow;
            AllProjectileEvents = allProjectileEvents;
            WeaponChanger = weaponChanger;
        }
    }
}