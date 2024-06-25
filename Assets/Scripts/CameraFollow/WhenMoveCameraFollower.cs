using System;
using BattleStateMachineComponents.StatesData;
using UltimateCC;
using WormComponents;

namespace CameraFollow
{
    public class WhenMoveCameraFollower
    {
        private readonly FollowingCamera _followingCamera;
        private readonly IMovementInput _movementInput;
        private readonly Worm _currentWorm;

        public WhenMoveCameraFollower(FollowingCamera followingCamera, IMovementInput movementInput, Worm currentWorm)
        {
            _followingCamera = followingCamera;
            _movementInput = movementInput;
            _currentWorm = currentWorm;
        }

        public void Enable()
        {
            _movementInput.WalkPerformed += OnWalkPerformed;
        }

        public void Disable()
        {
            if(_movementInput != null)
                _movementInput.WalkPerformed -= OnWalkPerformed;
        }

        private void OnWalkPerformed(float direction)
        {
            if(direction == 0)
                return;
            
            _followingCamera.SetTarget(_currentWorm.transform);
            Disable();
        }
    }
}