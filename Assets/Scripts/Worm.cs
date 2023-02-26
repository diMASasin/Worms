using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{

    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] float _speed = 2f;
    [SerializeField] float _jumpSpeed = 5f;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _wormSprite;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.velocity += new Vector2(0, _jumpSpeed);
            _animator.SetBool("Grounded", false);
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            _wormSprite.localScale = new Vector3(-horizontal, 1f,1f);
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = horizontal * _speed;
            _rigidbody.velocity = velocity;
            _animator.SetBool("Walk", true);
        }
        else {
            _animator.SetBool("Walk", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _animator.SetBool("Grounded", true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _animator.SetBool("Grounded", false);
    }

}
