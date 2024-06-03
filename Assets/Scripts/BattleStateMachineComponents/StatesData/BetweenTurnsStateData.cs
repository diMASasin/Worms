using System;
using Projectiles;
using UnityEngine;
using Wind_;

namespace BattleStateMachineComponents.StatesData
{
    [Serializable]
    public class BetweenTurnsStateData
    {
        public WindMediator WindMediator { get; private set; }

        public void Init(WindData data, WindView windView, IProjectileEvents projectileEvents)
        {
            WindMediator = new WindMediator(data, windView, projectileEvents);
        }
    }
}