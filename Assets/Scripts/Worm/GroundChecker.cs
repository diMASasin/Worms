using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private Vector2 _size;
    [SerializeField] private Vector2 _offset = new Vector2(0, -0.05f);
    [SerializeField] private bool _showGroundCheckerBox = true;
    [SerializeField] private ContactFilter2D _contactFilter2D;
    [SerializeField] private Worm _worm;

    private List<Collider2D> _contacts = new();

    public bool IsGrounded { get; private set; }

    public event UnityAction<bool> IsGroundedChanged;

    private void Start()
    {
        _size = new Vector2(0.3f, 0.01f);
    }

    void FixedUpdate()
    {
        Physics2D.OverlapBox(GetPoint(), _size, 0, _contactFilter2D, _contacts);

        if(_contacts.Contains(_worm.Collider2D))
            _contacts.Remove(_worm.Collider2D);
        IsGrounded = _contacts.Count > 0;

        IsGroundedChanged?.Invoke(IsGrounded);
    }

    private void OnDrawGizmos()
    {
        if(_showGroundCheckerBox)
            Gizmos.DrawCube(GetPoint(), _size);
    }

    private Vector2 GetPoint()
    {
        return (Vector2)transform.position + _offset;
    }
}
