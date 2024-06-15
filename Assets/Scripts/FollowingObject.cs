using System;
using UnityEngine;

[Serializable]
public class FollowingObject : IFixedTickable
{
    [SerializeField] private Transform _followingTransform;
    [SerializeField] private bool _moveSmoothly;
    [SerializeField] private float _speed = 10;
    [field: SerializeField] public Vector3 Offset { get; private set; }
    
    private Vector3 _newPosition;
    public Vector3 MoveTo { get; private set; }
    public Transform FollowingFor { get; private set; }
    public bool FreezeZPosition { get; private set; }
    private Vector3 CurrentPosition
    {
        get => _followingTransform.position;
        set => _followingTransform.position = value;
    }

    public void LateTick()
    {
        _newPosition = FollowingFor != null ? FollowingFor.position + Offset : MoveTo;

        if (FreezeZPosition == true && FollowingFor != null)
            _newPosition.z = _followingTransform.position.z;

        if (_moveSmoothly == true)
            CurrentPosition = Vector3.Lerp(CurrentPosition, _newPosition, _speed * Time.deltaTime);
        else
            _followingTransform.position = _newPosition;
    }

    public void Follow(Transform target)
    {
        if (target == null)
            return;

        FreezeZPosition = false;
        FollowingFor = target;
    }

    public void Follow(Vector3 newPosition)
    {
        FreezeZPosition = false;
        MoveTo = newPosition;
    }

    public void StopFollowZPosition()
    {
        FreezeZPosition = true;
    }

    public void StopFollow()
    {
        FollowingFor = null;
    }
}