using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GroundChecker : IFixedTickable
{
    private Collider2D _collider;
    private List<Collider2D> _contacts = new();
    private Transform _transform;
    private GroundCheckerConfig _config;

    public bool IsGrounded { get; private set; }

    public event UnityAction<bool> IsGroundedChanged;

    public GroundChecker(Transform transform, Collider2D collider, GroundCheckerConfig config)
    {
        _transform = transform;
        _collider = collider;
        _config = config;

        MonoBehaviourPerformer.AddFixedTickable(this);
    }

    public void FixedTick()
    {
        Physics2D.OverlapBox(GetPoint(), _config.Size, 0, _config.ContactFilter2D, _contacts);

        if(_contacts.Contains(_collider))
            _contacts.Remove(_collider);
        IsGrounded = _contacts.Count > 0;

        IsGroundedChanged?.Invoke(IsGrounded);
    }

    private void OnDrawGizmos()
    {
        if(_config.ShowGroundCheckerBox)
            Gizmos.DrawCube(GetPoint(), _config.Size);
    }

    private Vector2 GetPoint()
    {
        return (Vector2)_transform.position + _config.Offset;
    }
}
