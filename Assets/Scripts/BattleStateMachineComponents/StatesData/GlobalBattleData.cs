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
    public class GlobalBattleData : ICurrentWorm
    {
        [field: SerializeField] public FollowingCamera FollowingCamera { get; private set; }
        [field: SerializeField] public Water Water { get; private set; }
        
        public TimersConfig TimersConfig { get; private set; }
        public CycledList<Team> AliveTeams { get; private set; } = new();
        
        public Worm CurrentWorm { get; set; }
        public Team CurrentTeam { get; set; }

        public Timer GlobalTimer;
        public Timer TurnTimer;

        public IMovementInput MovementInput { get; private set; }
        
        public void Init(TimersConfig timersConfig, IMovementInput movementInput)
        {
            MovementInput = movementInput;
            TimersConfig = timersConfig;
            
            GlobalTimer = new Timer();
            TurnTimer = new Timer();
        }
    }
}