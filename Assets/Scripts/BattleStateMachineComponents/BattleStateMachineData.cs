using System;
using BattleStateMachineComponents.StatesData;
using UI;
using UnityEngine;

namespace BattleStateMachineComponents
{
    [Serializable]
    public class BattleStateMachineData
    {
        [field: SerializeField] public GlobalBattleData GlobalBattleData { get; private set; }
        [field: SerializeField] public StartStateData StartStateData { get; private set; }
        [field: SerializeField] public BetweenTurnsStateData BetweenTurnsData { get; private set; }
        [field: SerializeField] public TurnStateData TurnStateData { get; private set; }
        [field: SerializeField] public EndScreen EndScreen { get; private set; }
    }
}