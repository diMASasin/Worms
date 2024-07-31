using System;
using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Configs;
using DestructibleLand;
using Timers;
using _UI;
using UnityEngine;
using Wind_;
using WormComponents;

namespace BattleStateMachineComponents
{
    [Serializable]
    public class BattleStateMachineData : ICurrentWorm, IAliveTeams, ITimers
    {
        [field: SerializeField] public CinemachineFollowingCamera CinemachineFollowingCamera { get; private set; }
        [field: SerializeField] public WaterLevelIncreaser WaterLevelIncreaser { get; private set; }
        [field: SerializeField] public UI UI { get; private set; }
        [field: SerializeField] public TerrainWrapper Terrain { get; private set; }
        [field: SerializeField] public ParticleSystem WindEffect { get; private set; }
        [field: SerializeField] public BattleConfig BattleConfig { get; private set; }

        public CycledList<Team> AliveTeams { get; set; }
        
        public Worm CurrentWorm { get; set; }
        public Team CurrentTeam { get; set; }

        public Timer BattleTimer { get; set; }
        public Timer TurnTimer { get; set; }
    }
}