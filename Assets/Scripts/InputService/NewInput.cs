using System;
using UltimateCC;
using static UnityEngine.InputSystem.InputAction;

namespace InputService
{
    public class MovementInput : IInput
    {
        private readonly InputActions _playerControls = new();
        
        public event Action<bool> LongJumpStarted;
        public event Action<bool> HighJumpStarted;
        public event Action<bool> JumpCanceled;
        public event Action<float> WalkPerformed;
        public event Action<bool> DashPerformed;
        public event Action<bool> CrouchPerformed;
        public event Action<bool> WallGrabPerformed;
        public event Action<float> WallClimbPerformed;
        
        public void Enable() => _playerControls.Enable();

        public void Disable() => _playerControls.Disable();

        public void Subscribe()
        {
            _playerControls.Player.Jump.started += OnLongJumpStarted;
            _playerControls.Player.Highjump.started += OnHighJumpStarted;
            _playerControls.Player.Jump.canceled += OnHighJumpCanceled;
            _playerControls.Player.Walk.performed += OnWalkPerformed;
            _playerControls.Player.Walk.canceled += OnWalkCanceled;
            _playerControls.Player.Dash.performed += OnDashPerformed;
            _playerControls.Player.Crouch.performed += OnCrouchPerformed;
            _playerControls.Player.WallGrab.performed += OnWallGrabPerformed;
            _playerControls.Player.WallClimb.performed += OnWallClimbPerformed;
        }
        
        public void Unsubscribe()
        {
            _playerControls.Player.Jump.started -= OnLongJumpStarted;
            _playerControls.Player.Highjump.started -= OnHighJumpStarted;
            _playerControls.Player.Jump.canceled -= OnHighJumpCanceled;
            _playerControls.Player.Walk.performed -= OnWalkPerformed;
            _playerControls.Player.Dash.performed -= OnDashPerformed;
            _playerControls.Player.Crouch.performed -= OnCrouchPerformed;
            _playerControls.Player.WallGrab.performed -= OnWallGrabPerformed;
            _playerControls.Player.WallClimb.performed -= OnWallClimbPerformed;
        }

        private void OnLongJumpStarted(CallbackContext context) => 
            LongJumpStarted?.Invoke(context.ReadValueAsButton());
        private void OnHighJumpStarted(CallbackContext context) => 
            HighJumpStarted?.Invoke(context.ReadValueAsButton());
        private void OnHighJumpCanceled(CallbackContext context) => 
            JumpCanceled?.Invoke(context.ReadValueAsButton());
        private void OnWalkPerformed(CallbackContext context) => 
            WalkPerformed?.Invoke(context.ReadValue<float>());
        private void OnWalkCanceled(CallbackContext obj) => 
            WalkPerformed?.Invoke(0);
        private void OnDashPerformed(CallbackContext context) => 
            DashPerformed?.Invoke(context.ReadValueAsButton());
        private void OnCrouchPerformed(CallbackContext context) => 
            CrouchPerformed?.Invoke(context.ReadValueAsButton());
        private void OnWallGrabPerformed(CallbackContext context) => 
            WallGrabPerformed?.Invoke(context.ReadValueAsButton());
        private void OnWallClimbPerformed(CallbackContext context) => 
            WallClimbPerformed?.Invoke(context.ReadValue<float>());
    }
}