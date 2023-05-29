using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormAnimations : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private WormMovement _wormMovement;

    private void OnEnable()
    {
        _groundChecker.IsGroundedChanged += OnGroundedChanged;
        _wormMovement.IsWalkingChanged += OnWalkingChanged;
    }

    private void OnDisable()
    {
        _groundChecker.IsGroundedChanged -= OnGroundedChanged;
        _wormMovement.IsWalkingChanged -= OnWalkingChanged;
    }

    private void OnGroundedChanged(bool isGrounded)
    {
        _animator.SetBool("Grounded", isGrounded);
    }

    private void OnWalkingChanged(bool isWalking)
    {
        _animator.SetBool("Walk", isWalking);
    }
}
