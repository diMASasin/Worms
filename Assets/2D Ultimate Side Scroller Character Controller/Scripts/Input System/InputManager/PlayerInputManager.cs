using UnityEngine;
using static UltimateCC.PlayerMain;

namespace UltimateCC
{
    public class InputHandler : MonoBehaviour
    {
        private IMovementInput _playerMovementInput;
        private PlayerMain player;

        // Variables to store input
        [SerializeField, NonEditable] private float input_Walk;
        [SerializeField, NonEditable] private bool input_Jump;
        [SerializeField, NonEditable] private bool input_Dash;
        [SerializeField, NonEditable] private bool input_Crouch;
        [SerializeField, NonEditable] private bool input_WallGrab;
        [SerializeField, NonEditable] private float input_WallClimb;

        public PlayerData PlayerData { get; private set; }
        
        // Properties to access the input variables
        public float Input_Walk => input_Walk;
        public bool Input_Jump => input_Jump;
        public bool Input_Dash => input_Dash;
        public bool Input_Crouch => input_Crouch;
        public bool Input_WallGrab => input_WallGrab;
        public float Input_WallClimb => input_WallClimb;

        private void Awake()
        {
            player = GetComponent<PlayerMain>(); // Reference for Ultimate2DPlayer component where all of content come up together
            PlayerData = player.PlayerData; // Reference for Ultimate2DPlayer.PlayerData component where all variables stored
        }

        public void Enable(IMovementInput movementInput)
        {
            _playerMovementInput = movementInput;
            
            _playerMovementInput.Enable();
            _playerMovementInput.LongJumpStarted += OnLongJumpStarted;
            _playerMovementInput.HighJumpStarted += OnHighJumpStarted;
            _playerMovementInput.WalkPerformed += OnWalk;
            _playerMovementInput.DashPerformed += OnDash;
            _playerMovementInput.CrouchPerformed += OnCrouch;
            _playerMovementInput.WallGrabPerformed += OnWallGrab;
            _playerMovementInput.WallClimbPerformed += OnWallClimb;
        }

        public void Disable()
        {
            if (_playerMovementInput == null)
                return;
            OnWalk(0);
            
            _playerMovementInput.LongJumpStarted -= OnLongJumpStarted;
            _playerMovementInput.HighJumpStarted -= OnHighJumpStarted;
            _playerMovementInput.WalkPerformed -= OnWalk;
            _playerMovementInput.DashPerformed -= OnDash;
            _playerMovementInput.CrouchPerformed -= OnCrouch;
            _playerMovementInput.WallGrabPerformed -= OnWallGrab;
            _playerMovementInput.WallClimbPerformed -= OnWallClimb;
            
            _playerMovementInput.Disable();
            _playerMovementInput = null;
        }

        private void FixedUpdate() => UpdateTimers();

        private void UpdateTimers()
        {
            //coyote time and jump buffer timers implementation
            if (PlayerData.Physics.IsGrounded && player.CurrentState != AnimName.Jump && (!PlayerData.Physics.IsOnNotWalkableSlope || PlayerData.Physics.IsMultipleContactWithNonWalkableSlope))
            {
                PlayerData.Jump.CoyoteTimeTimer = PlayerData.Jump.CoyoteTimeMaxTime;
            }
            else if (player.CurrentState == AnimName.Jump)
            {
                PlayerData.Jump.CoyoteTimeTimer = 0f;
            }

            if (PlayerData.Physics.IsNextToWall && player.CurrentState != AnimName.WallJump)
            {
                PlayerData.Walls.WallJump.CoyoteTimeTimer = PlayerData.Walls.WallJump.CoyoteTimeMaxTime;
            }
            else if (player.CurrentState == AnimName.WallJump)
            {
                PlayerData.Walls.WallJump.CoyoteTimeTimer = 0f;
            }

            PlayerData.Jump.JumpBufferTimer = PlayerData.Jump.JumpBufferTimer > 0f ? PlayerData.Jump.JumpBufferTimer - Time.deltaTime : 0f;
            PlayerData.Jump.CoyoteTimeTimer = PlayerData.Jump.CoyoteTimeTimer > 0f ? PlayerData.Jump.CoyoteTimeTimer - Time.deltaTime : 0f;
            PlayerData.Dash.DashCooldownTimer = PlayerData.Dash.DashCooldownTimer > 0f ? PlayerData.Dash.DashCooldownTimer - Time.deltaTime : 0f;
            PlayerData.Walls.WallJump.JumpBufferTimer = PlayerData.Walls.WallJump.JumpBufferTimer > 0f ? PlayerData.Walls.WallJump.JumpBufferTimer - Time.deltaTime : 0f;
            PlayerData.Walls.WallJump.CoyoteTimeTimer = PlayerData.Walls.WallJump.CoyoteTimeTimer > 0f ? PlayerData.Walls.WallJump.CoyoteTimeTimer - Time.deltaTime : 0f;
        }

        // all input based events

        #region Input Event Functions

        private void OnHighJumpStarted(bool value)
        {
            PlayerData.Jump.currentJumpType = PlayerData.JumpVariables.JumpType.HighJump;
            OnJumpStarted(value);
        }
        
        private void OnLongJumpStarted(bool value)
        {
            PlayerData.Jump.currentJumpType = PlayerData.JumpVariables.JumpType.LongJump;
            OnJumpStarted(value);
        }

        private void OnJumpStarted(bool value)
        {
            input_Jump = value;
            PlayerData.Jump.JumpBufferTimer = PlayerData.Jump.JumpBufferMaxTime;
            PlayerData.Walls.WallJump.JumpBufferTimer = PlayerData.Walls.WallJump.JumpBufferMaxTime;
            PlayerData.Jump.NewJump = player.CurrentState != AnimName.WallGrab
                                    && player.CurrentState != AnimName.WallSlide
                                    && player.CurrentState != AnimName.WallClimb
                                    && PlayerData.Walls.WallJump.CoyoteTimeTimer == 0;
        }

        private void OnWalk(float value)
        {
            input_Walk = value;
        }

        private void OnDash(bool value)
        {
            input_Dash = value;
        }

        private void OnCrouch(bool value)
        {
            input_Crouch = value;
        }
        private void OnWallGrab(bool value)
        {
            input_WallGrab = value;
        }
        private void OnWallClimb(float value)
        {
            input_WallClimb = value;
        }
        #endregion
    }
}
