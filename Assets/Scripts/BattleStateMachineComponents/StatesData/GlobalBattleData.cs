using System;
using CameraFollow;
using Configs;
using EventProviders;
using InputService;
using Timers;
using UltimateCC;
using UnityEngine;
using WormComponents;

namespace BattleStateMachineComponents.StatesData
{
    [Serializable]
    public class GlobalBattleData
    {
        [field: SerializeField] public FollowingCamera FollowingCamera { get; private set; }
        [field: SerializeField] public Water Water { get; private set; }
        
        public TimersConfig TimersConfig { get; private set; }
        public CycledList<Team> AliveTeams { get; private set; } = new();
        
        [NonSerialized] public Worm CurrentWorm;
        [NonSerialized] public Team CurrentTeam;
        
        public Timer GlobalTimer;
        public Timer TurnTimer;

        public IInput Input { get; private set; }
        
        public void Init(TimersConfig timersConfig, IInput input)
        {
            Input = input;
            TimersConfig = timersConfig;
            
            Input.Enable();
            GlobalTimer = new Timer();
            TurnTimer = new Timer();
        }
    }
}