using MovementComponents;
using UnityEngine;

namespace WormComponents
{
    public class WormAnimations : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private GroundChecker _groundChecker;
        [SerializeField] private Movement _movement;
    
        private static readonly int Grounded = Animator.StringToHash("Grounded");
        private static readonly int Walk = Animator.StringToHash("Walk");

        private void OnEnable()
        {
            _groundChecker.IsGroundedChanged += OnGroundedChanged;
            _movement.IsWalkingChanged += OnWalkingChanged;
        }

        private void OnDisable()
        {
            if(_groundChecker  != null)
                _groundChecker.IsGroundedChanged -= OnGroundedChanged;

            if(_movement != null)
                _movement.IsWalkingChanged -= OnWalkingChanged;
        }

        private void OnGroundedChanged(bool isGrounded)
        {
            _animator.SetBool(Grounded, isGrounded);
        }

        private void OnWalkingChanged(bool isWalking)
        {
            _animator.SetBool(Walk, isWalking);
        }
    }
}
