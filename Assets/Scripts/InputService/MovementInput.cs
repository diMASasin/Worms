using MovementComponents;
using UnityEngine.InputSystem;

namespace InputService
{
    public class MovementInput
    {
        private readonly MainInput.MovementActions _movementInput;

        private IMovement _movement;

        public MovementInput(MainInput.MovementActions movementInput)
        {
            _movementInput = movementInput;
        }
        
        public void Enable(IMovement movement)
        {
            _movement = movement;
            _movementInput.Enable();
            
            _movementInput.TurnRight.performed += OnTurnRight;
            _movementInput.TurnLeft.performed += OnTurnLeft;
            _movementInput.LongJump.performed += OnLongJump;
            _movementInput.HighJump.performed += OnHighJump;
        }

        public void Disable()
        {
            if(_movement == null)
                return;
            _movementInput.Disable();
            
            _movementInput.TurnRight.performed -= OnTurnRight;
            _movementInput.TurnLeft.performed -= OnTurnLeft;
            _movementInput.LongJump.performed -= OnLongJump;
            _movementInput.HighJump.performed -= OnHighJump;
        }

        public void Tick()
        {
            OnDirectionChanged();
        }

        private void OnHighJump(InputAction.CallbackContext obj)
        {
            _movement.HighJump();
        }

        private void OnLongJump(InputAction.CallbackContext obj)
        {
            _movement.LongJump();
        }

        private void OnTurnRight(InputAction.CallbackContext obj)
        {
            _movement.TurnRight();
        }

        private void OnTurnLeft(InputAction.CallbackContext obj)
        {
            _movement.TurnLeft();
        }

        private void OnDirectionChanged()
        {
            if(_movement == null || CanMove() == false)
                return;

            var direction = _movementInput.Move.ReadValue<float>();
            _movement.TryMove(direction);
        }

        private bool CanMove()
        {
            return _movementInput.DontMove.ReadValue<float>() == 0;
        }
    }
}