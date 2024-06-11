using System;
using BattleStateMachineComponents.StatesData;
using UltimateCC;

namespace CameraFollow
{
    public class WormWhenMoveCameraFollower
    {
        private readonly FollowingCamera _followingCamera;
        private readonly IMovementInput _movementInput;
        private readonly ICurrentWorm _currentWorm;

        public WormWhenMoveCameraFollower(FollowingCamera followingCamera, IMovementInput movementInput,
            ICurrentWorm currentWorm)
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
            
            _followingCamera.SetTarget(_currentWorm.CurrentWorm.transform);
            Disable();
        }
    }
}