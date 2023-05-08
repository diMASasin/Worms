using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class WormMovement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _wormArmature;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private WormInput _wormInput;
    [SerializeField] private float _speed;
    [SerializeField] private float _longJumpForceX = 2;
    [SerializeField] private float _longJumpForceY = 2;
    [SerializeField] private float _highJumpForceX = 2;
    [SerializeField] private float _highJumpForceY = 2;
    [SerializeField] private float _jumpCooldown = 0.5f;

    private bool _canJump = true;

    public event UnityAction<bool> IsWalkingChanged;

    private void OnEnable()
    {
        _wormInput.InputDisabled += OnInputDisabled;
    }

    private void OnDisable()
    {
        _wormInput.InputDisabled -= OnInputDisabled;
    }

    private void FixedUpdate()
    {
        if (!_groundChecker.IsGrounded && _rigidbody.bodyType == RigidbodyType2D.Kinematic)
            _rigidbody.velocity += (Vector2)Physics.gravity * Time.deltaTime;
        else if (_groundChecker.IsGrounded && _rigidbody.bodyType == RigidbodyType2D.Kinematic)
            _rigidbody.velocity = Vector2.zero;
    }

    private void OnInputDisabled()
    {
        IsWalkingChanged?.Invoke(false);
    }

    public void TryMove(float horizontal)
    {
        if (horizontal != 0 && _groundChecker.IsGrounded)
        {
            _wormArmature.transform.right = new Vector3(-horizontal, 0);
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = horizontal * _speed;
            _rigidbody.velocity = velocity;
        }

        IsWalkingChanged?.Invoke(horizontal != 0);
    }

    private bool TryJump()
    {
        if (!_groundChecker.IsGrounded || !_canJump)
            return false;

        _canJump = false;
        StartCoroutine(JumpCooldown(_jumpCooldown));

        return true;
    }

    public void LongJump()
    {
        if (!TryJump())
            return;
        _rigidbody.velocity += new Vector2(_longJumpForceX * -_wormArmature.transform.right.x, _longJumpForceY);
    }

    public void HighJump()
    {
        if (!TryJump())
            return;
        _rigidbody.velocity += new Vector2(_highJumpForceX * _wormArmature.transform.right.x, _highJumpForceY);
    }

    private IEnumerator JumpCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        _canJump = true;
    }
}
