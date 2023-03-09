using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WormMovement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _sprite;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _jumpCooldown = 0.5f;

    private bool _canJump = true;

    public event UnityAction<bool> IsWalkingChanged;

    private void FixedUpdate()
    {
        if (!_groundChecker.IsGrounded && _rigidbody.bodyType == RigidbodyType2D.Kinematic)
            _rigidbody.velocity += (Vector2)Physics.gravity * Time.deltaTime;
        else if (_groundChecker.IsGrounded && _rigidbody.bodyType == RigidbodyType2D.Kinematic)
            _rigidbody.velocity = Vector2.zero;
    }

    public void TryMove(float horizontal)
    {
        if (horizontal != 0)
        {
            _sprite.localScale = new Vector3(-horizontal, 1f, 1f);
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = horizontal * _speed;
            _rigidbody.velocity = velocity;
        }

        IsWalkingChanged?.Invoke(horizontal != 0);
    }

    public void Jump()
    {
        if (!_groundChecker.IsGrounded || !_canJump)
            return;

        _rigidbody.velocity += new Vector2(0, _jumpSpeed);

        _canJump = false;
        StartCoroutine(JumpCooldown(_jumpCooldown));
    }

    private IEnumerator JumpCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        _canJump = true;
    }
}
