using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundChecker : MonoBehaviour
{
    private Vector2 _size;

    public bool IsGrounded { get; private set; }

    public event UnityAction<bool> IsGroundedChanged;

    private void Start()
    {
        _size = new Vector2(0.3f, 0.01f);
    }

    void FixedUpdate()
    {
        IsGrounded = Physics2D.OverlapBox(GetPoint(), _size, 0);
        IsGroundedChanged?.Invoke(IsGrounded);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(GetPoint(), _size);
    }

    private Vector2 GetPoint()
    {
        return (Vector2)transform.position - new Vector2(0, 0.05f);
    }
}
