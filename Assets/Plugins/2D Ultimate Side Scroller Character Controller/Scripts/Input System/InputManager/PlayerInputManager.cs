using UltimateCC;
using UnityEngine;
using UnityEngine.InputSystem;
using static UltimateCC.PlayerMain;

namespace UltimateCC
{
    public class PlayerInputManager : MonoBehaviour
    {
        // Declaration of necessary references
        private PlayerMain player;
        public PlayerData PlayerData { get; private set; }
        public InputActions playerControls;

        // Variables to store input
        [SerializeField, NonEditable] private float input_Walk;
        [SerializeField, NonEditable] private bool input_Jump;
        [SerializeField, NonEditable] private bool input_Dash;
        [SerializeField, NonEditable] private bool input_Crouch;
        [SerializeField, NonEditable] private bool input_WallGrab;
        [SerializeField, NonEditable] private float input_WallClimb;

        // Properties to access the input variables
        public float Input_Walk => input_Walk;
        public bool Input_Jump => input_Jump;
        public bool Input_Dash => input_Dash;
        public bool Input_Crouch => input_Crouch;
        public bool Input_WallGrab => input_WallGrab;
        public float Input_WallClimb => input_WallClimb;

        // Constructor to set player and playerData references
        public PlayerInputManager(PlayerMain player, PlayerData playerData)
        {
            this.player = player;
            this.PlayerData = playerData;
        }


        private void Awake()
        {
            playerControls = new InputActions(); // Initialize InputActions object to use input map of new input system
            player = GetComponent<PlayerMain>(); // Reference for Ultimate2DPlayer component where all of content come up together
            PlayerData = player.PlayerData; // Reference for Ultimate2DPlayer.PlayerData component where all variables stored
        }

        private void OnEnable()
        {
            // This code block assigns events to actions in order to receive input from the player.
            playerControls.Player.Enable();
            playerControls.Player.Jump.started += OnLongJumpStarted;
            playerControls.Player.Highjump.started += OnHighJumpStarted;
            playerControls.Player.Jump.canceled += OnJumpCanceled;
            playerControls.Player.Walk.performed += OnWalk;
            playerControls.Player.Walk.canceled += OnWalk;
            playerControls.Player.Dash.performed += OnDash;
            playerControls.Player.Dash.canceled += OnDash;
            playerControls.Player.Crouch.performed += OnCrouch;
            playerControls.Player.Crouch.canceled += OnCrouch;
            playerControls.Player.WallGrab.performed += OnWallGrab;
            playerControls.Player.WallGrab.canceled += OnWallGrab;
            playerControls.Player.WallClimb.performed += OnWallClimb;
            playerControls.Player.WallClimb.canceled += OnWallClimb;
        }

        public void Enable()
        {
            playerControls.Enable();
        }

        public void Disable()
        {
            playerControls.Disable();
        }

        private void FixedUpdate()
        {
            UpdateTimers();
        }

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

        private void OnHighJumpStarted(InputAction.CallbackContext context)
        {
            PlayerData.Jump.currentJumpType = PlayerData.JumpVariables.JumpType.HighJump;
            OnJumpStarted(context);
        }
        
        private void OnLongJumpStarted(InputAction.CallbackContext context)
        {
            PlayerData.Jump.currentJumpType = PlayerData.JumpVariables.JumpType.LongJump;
            OnJumpStarted(context);
        }

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            input_Jump = context.ReadValueAsButton();
            PlayerData.Jump.JumpBufferTimer = PlayerData.Jump.JumpBufferMaxTime;
            PlayerData.Walls.WallJump.JumpBufferTimer = PlayerData.Walls.WallJump.JumpBufferMaxTime;
            PlayerData.Jump.NewJump = player.CurrentState != AnimName.WallGrab
                                    && player.CurrentState != AnimName.WallSlide
                                    && player.CurrentState != AnimName.WallClimb
                                    && PlayerData.Walls.WallJump.CoyoteTimeTimer == 0;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            // input_Jump = context.ReadValueAsButton();
            // if (player.CurrentState == AnimName.Jump)
            // {
                // PlayerData.Physics.CutJump = true;
            // }
        }

        private void OnWalk(InputAction.CallbackContext context)
        {
            input_Walk = context.ReadValue<float>();
        }

        private void OnDash(InputAction.CallbackContext context)
        {
            input_Dash = context.ReadValueAsButton();
        }

        private void OnCrouch(InputAction.CallbackContext context)
        {
            input_Crouch = context.ReadValueAsButton();
        }
        private void OnWallGrab(InputAction.CallbackContext context)
        {
            input_WallGrab = context.ReadValueAsButton();
        }
        private void OnWallClimb(InputAction.CallbackContext context)
        {
            input_WallClimb = context.ReadValue<float>();
        }
        #endregion
    }
}
