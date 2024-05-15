using UnityEngine;

public class WormAnimations : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private GroundChecker _groundChecker;
    private Movement _movement;
    
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int Walk = Animator.StringToHash("Walk");

    public void Init(GroundChecker groundChecker, Movement movement)
    {
        _groundChecker = groundChecker;
        _movement = movement;

        _groundChecker.IsGroundedChanged += OnGroundedChanged;
        _movement.IsWalkingChanged += OnWalkingChanged;
    }

    private void OnDestroy()
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
