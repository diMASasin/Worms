using System.Collections.Generic;
using Configs;
using EventProviders;
using Projectiles;
using Timers;
using UnityEngine;
using Weapons;
using Input = PlayerInput.Input;

namespace GameBattleStateMachine
{
    public class BattleStateMachineData
    {
        public readonly TimersConfig TimersConfig;
        public readonly FollowingCamera FollowingCamera;
        public readonly EndScreen EndScreen;
        public readonly Input Input;
        public readonly Transform GeneralView;
        public readonly Timer TurnTimer;
        public readonly Timer GlobalTimer;
        public readonly List<Team> AliveTeams;
        public readonly Arrow Arrow;
        public readonly WeaponSelector WeaponSelector;
        public readonly WeaponView WeaponView;
        public readonly Wind Wind;
        public readonly WeaponChanger WeaponChanger;
        public readonly WaterMediator WaterMediator;
        public readonly ProjectileLauncher ProjectileLauncher;

        public int CurrentTeamIndex = -1;
        public Worm CurrentWorm;
        public Team CurrentTeam;
        
        public BattleStateMachineData(TimersConfig timersConfig, FollowingCamera followingCamera, EndScreen endScreen,
            Input input, Transform generalView, Timer turnTimer, Timer globalTimer, List<Team> aliveTeams, Arrow arrow,
            WeaponSelector weaponSelector, WeaponView weaponView, Wind wind, WeaponChanger weaponChanger,
            WaterMediator waterMediator, ProjectileLauncher projectileLauncher)
        {
            TimersConfig = timersConfig;
            FollowingCamera = followingCamera;
            EndScreen = endScreen;
            Input = input;
            GeneralView = generalView;
            TurnTimer = turnTimer;
            GlobalTimer = globalTimer;
            AliveTeams = aliveTeams;
            Arrow = arrow;
            WeaponSelector = weaponSelector;
            WeaponView = weaponView;
            Wind = wind;
            WeaponChanger = weaponChanger;
            WaterMediator = waterMediator;
            ProjectileLauncher = projectileLauncher;
        }
        
        public bool TryGetCurrentTeam(out Team team)
        {
            if (CurrentTeamIndex >= AliveTeams.Count || CurrentTeamIndex < 0)
                team = null;
            else
                team = AliveTeams[CurrentTeamIndex];

            return team != null;
        }
        
        public bool TryGetNextTeam(out Team team)
        {
            CurrentTeamIndex++;

            if (CurrentTeamIndex >= AliveTeams.Count)
                CurrentTeamIndex = 0;

            TryGetCurrentTeam(out team);

            return team != null;
        }
    }
}