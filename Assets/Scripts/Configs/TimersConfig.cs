using System;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class TimersConfig
    {
        [field: SerializeField] public float TurnDuration { get; private set; } = 6;
        [field: SerializeField] public float RetreatDuration { get; private set; } = 5;
        [field: SerializeField] public float BetweenTurnsDuration { get; private set; } = 3;
        [field: SerializeField] public float BattleTime { get; private set; } = 900;
    }
}