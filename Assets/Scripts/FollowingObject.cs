using UnityEngine;

public class FollowingObject : IFixedTickable
{
    private readonly Vector2 _offset;
    private readonly Transform _objectTransform;
    private Transform _followingFor;

    public FollowingObject(Transform objectTransform, Vector2 offset)
    {
        _offset = offset;
        _objectTransform = objectTransform;
    }

    public void LateTick()
    {
        if(_followingFor != null)
            _objectTransform.position = _followingFor.position + (Vector3)_offset;
    }

    public void Follow(Transform target)
    {
        _followingFor = target;
    }

    public void Disonnect()
    {
        _followingFor = null;
    }
}
