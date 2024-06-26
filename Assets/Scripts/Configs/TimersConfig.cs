using System;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class TimersConfig
    {
        [field: SerializeField] public float TurnDuration { get; private set; } = 6f;
        [field: SerializeField] public float RetreatDuration { get; private set; } = 5f;
        [field: SerializeField] public float ProjectileWaitingDuration { get; private set; } = 2f;
        [field: SerializeField] public float BetweenTurnsDuration { get; private set; } = 3f;
        [field: SerializeField] public float BattleTime { get; private set; } = 900f;
    }
}