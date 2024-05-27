using System;
using CameraFollow;
using Configs;
using InputService;
using Timers;
using UnityEngine;
using WormComponents;

namespace BattleStateMachineComponents
{
    public class GlobalBattleData
    {
        [field: SerializeField] public FollowingCamera FollowingCamera { get; private set; }
        [field: SerializeField] public TimersConfig TimersConfig { get; private set; }
        
        [NonSerialized] public IWorm CurrentWorm;
        [NonSerialized] public Team CurrentTeam;
        
        public readonly Timer TurnTimer = new();
        
        public PlayerInput PlayerInput;
        public MainInput MainInput { get; private set; }

        public void Init(MainInput mainInput, TimersConfig timersConfig)
        {
            MainInput = mainInput;
            TimersConfig = timersConfig;
        }
    }
}