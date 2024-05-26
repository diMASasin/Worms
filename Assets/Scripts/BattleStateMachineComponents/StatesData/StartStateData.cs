using System;
using Configs;
using DestructibleLand;
using Spawn;
using Timers;
using UI;
using UnityEngine;
using Wind_;

namespace BattleStateMachineComponents.StatesData
{
    [Serializable]
    public class StartStateData
    {
        [field: SerializeField] public WindView WindView { get; private set; }
        [field: SerializeField] public UIChanger UIChanger { get; private set; }
        [field: SerializeField] public TimerView TurnTimerView { get; private set; }
        [field: SerializeField] public TimerView GlobalTimerView { get; private set; }
        [field: SerializeField] public WormsSpawner WormsSpawner { get; private set; }
        [field: SerializeField] public TerrainWrapper Terrain { get; private set; }
        [field: SerializeField] public GameConfig GameConfig { get; private set; }
    }
}