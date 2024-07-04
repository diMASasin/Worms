using System;
using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Cinemachine;
using Configs;
using DestructibleLand;
using Timers;
using UI;
using UnityEngine;
using Wind_;
using WormComponents;

namespace BattleStateMachineComponents
{
    [Serializable]
    public class BattleStateMachineData : ICurrentWorm
    {
        [field: SerializeField] public CinemachineFollowingCamera CinemachineFollowingCamera { get; private set; }
        [field: SerializeField] public WaterLevelIncreaser WaterLevelIncreaser { get; private set; }
        [field: SerializeField] public WindView WindView { get; private set; }
        [field: SerializeField] public UIChanger UIChanger { get; private set; }
        [field: SerializeField] public TimerView TurnTimerView { get; private set; }
        [field: SerializeField] public TimerView GlobalTimerView { get; private set; }
        [field: SerializeField] public TerrainWrapper Terrain { get; private set; }
        [field: SerializeField] public BattleConfig BattleConfig { get; private set; }
        [field: SerializeField] public WeaponSelector WeaponSelector { get; private set; }
        [field: SerializeField] public EndScreen EndScreen { get; private set; }
        [field: SerializeField] public StylizedWater.Scripts.StylizedWater StylizedWater { get; private set; }

        public CycledList<Team> AliveTeams { get; set; }
        
        public Worm CurrentWorm { get; set; }
        public Team CurrentTeam { get; set; }

        public Timer BattleTimer;
        public Timer TurnTimer;
    }
}