using System;
using UnityEngine;

namespace UltimateCC
{
    public class PlayerMain : MonoBehaviour
    {
        private PlayerStateMachine _stateMachine; // State Machine declaration where we change current state
        [NonEditable, Space(5)] public AnimName CurrentState; // Variable to display the current state in the Unity inspector for debugging purposes.
        public MainState IdleState, WalkState, JumpState, LandState, DashState, CrouchIdleState, CrouchWalkState, WallGrabState, WallClimbState, WallJumpState, WallSlideState; // State declarations
        public enum AnimName { Idle, Walk, Jump, ExtraJump1, ExtraJump2, Land, Dash, CrouchIdle, CrouchWalk, WallGrab, WallClimb, WallJump, WallSlide } // Enum declaration of state names as animator parameters

        [NonSerialized] public Animator Animator; // The Animator is used to control the player's animations based on their current state.
        [NonSerialized] public Rigidbody2D Rigidbody2D; // The Rigidbody2D is used to control movement based on velocity vector.
        [NonSerialized] public PlayerInputManager InputManager; // The PlayerInputManager handles all user input and sends it to the state machine.
        [NonSerialized] public CapsuleCollider2D CapsuleCollider2D; // CapsuleCollider2D is used to handle slopes and define the ground check position in the base state class: "State.cs".

        public PlayerData PlayerData; // All player movement and action data is stored in the PlayerData object.

        private void Awake()
        {
            // Declaration of necessary components:
            // Animator for controlling character animations,
            // Rigidbody2D for physics simulation,
            // InputManager for handling player input,
            // CapsuleCollider2D for slope detection and ground checking.
            Animator = GetComponent<Animator>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
            InputManager = GetComponent<PlayerInputManager>();
            CapsuleCollider2D = GetComponent<CapsuleCollider2D>();

            // In this section, we assign all states
            _stateMachine = new PlayerStateMachine();
            IdleState = new PlayerIdleState(this, _stateMachine, AnimName.Idle, PlayerData);
            WalkState = new PlayerWalkState(this, _stateMachine, AnimName.Walk, PlayerData);
            JumpState = new PlayerJumpState(this, _stateMachine, AnimName.Jump, PlayerData);
            LandState = new PlayerLandState(this, _stateMachine, AnimName.Land, PlayerData);
            DashState = new PlayerDashState(this, _stateMachine, AnimName.Dash, PlayerData);
            CrouchIdleState = new PlayerCrouchIdleState(this, _stateMachine, AnimName.CrouchIdle, PlayerData);
            CrouchWalkState = new PlayerCrouchWalkState(this, _stateMachine, AnimName.CrouchWalk, PlayerData);
            WallGrabState = new PlayerWallGrabState(this, _stateMachine, AnimName.WallGrab, PlayerData);
            WallClimbState = new PlayerWallClimbState(this, _stateMachine, AnimName.WallClimb, PlayerData);
            WallJumpState = new PlayerWallJumpState(this, _stateMachine, AnimName.WallJump, PlayerData);
            WallSlideState = new PlayerWallSlideState(this, _stateMachine, AnimName.WallSlide, PlayerData);

            foreach (var jump in PlayerData.Jump.Jumps)
                jump.currentJumpType = jump.LongJump;
            
        }

        private void Start()
        {
            _stateMachine.Initialize(IdleState); //Here, we assign the starting state

            // The Derivative function is used to calculate the derivative of the animation curves,
            // which is necessary to obtain the velocity curve for the corresponding movement.
            // Here, we assign the derivative of each curve to its corresponding variable.
            // Basically, we get derivative of height curves to obtain velocity curves.
            
            // PlayerData.Jump.Jumps[0].JumpVelocityCurve = PlayerData.Jump.Jumps[0].JumpHeightCurve.Derivative();
            // foreach (var jump in PlayerData.Jump.Jumps)
            // {
            //     jump.JumpVelocityCurve = jump.JumpHeightCurve.Derivative();
            // }
            PlayerData.Land.LandVelocityCurve = PlayerData.Land.LandHeightCurve.Derivative();
            PlayerData.Dash.DashYVelocityCurve = PlayerData.Dash.DashHeightCurve.Derivative();
            PlayerData.Walls.WallJump.JumpVelocityCurve = PlayerData.Walls.WallJump.JumpHeightCurve.Derivative();
        }
        private void Update()
        {
            _stateMachine.CurrentState.Update(); // Update method of current state at runtime
        }

        private void FixedUpdate()
        {
            _stateMachine.CurrentState.FixedUpdate(); // FixedUpdate method of current state at runtime
        }
    }
}
