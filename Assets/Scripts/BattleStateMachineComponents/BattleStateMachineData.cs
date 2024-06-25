using System;
using BattleStateMachineComponents.StatesData;
using CameraFollow;
using Configs;
using DestructibleLand;
using EventProviders;
using Infrastructure;
using Projectiles;
using Timers;
using UI;
using UltimateCC;
using UnityEngine;
using Weapons;
using Wind_;
using WormComponents;
using Zenject;

namespace BattleStateMachineComponents
{
    [Serializable]
    public class BattleStateMachineData
    {
        [field: SerializeField] public FollowingCamera FollowingCamera { get; private set; }
        [field: SerializeField] public Water Water { get; private set; }
        [field: SerializeField] public WindView WindView { get; private set; }
        [field: SerializeField] public UIChanger UIChanger { get; private set; }
        [field: SerializeField] public TimerView TurnTimerView { get; private set; }
        [field: SerializeField] public TimerView GlobalTimerView { get; private set; }
        [field: SerializeField] public TerrainWrapper Terrain { get; private set; }
        [field: SerializeField] public GameConfig GameConfig { get; private set; }
        [field: SerializeField] public WeaponSelector WeaponSelector { get; private set; }
        [field: SerializeField] public EndScreen EndScreen { get; private set; }

        public TimersConfig TimersConfig { get; private set; } = new();
        public CycledList<Team> AliveTeams { get; set; }
        public WhenMoveCameraFollower WhenMoveCameraFollower { get; private set; }
        
        public Worm CurrentWorm { get; set; }
        public Team CurrentTeam { get; set; }

        public Timer BattleTimer;
        public Timer TurnTimer;
    }
}