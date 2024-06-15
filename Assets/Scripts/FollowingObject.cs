using System;
using UnityEngine;

[Serializable]
public class FollowingObject : IFixedTickable
{
    [SerializeField] private Transform _objectTransform;
    [SerializeField] private bool _moveSmoothly;
    [SerializeField] private float _speed = 10;
    [field: SerializeField] public Vector3 Offset { get; private set; }
    
    private Vector3 _newPosition;
    private Vector3 _moveTo;
    public Transform FollowingFor { get; private set; }
    public bool FreezeZPosition { get; private set; }
    private Vector3 CurrentPosition
    {
        get => _objectTransform.position;
        set => _objectTransform.position = value;
    }

    public void LateTick()
    {
        _newPosition = FollowingFor != null ? FollowingFor.position : _moveTo;

        if (FreezeZPosition == true)
            _newPosition.z = _objectTransform.position.z - Offset.z;

        if (_moveSmoothly == true)
            CurrentPosition = Vector3.Lerp(CurrentPosition, _newPosition + Offset, _speed * Time.deltaTime);
        else
            _objectTransform.position = _newPosition + Offset;
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
        _moveTo = newPosition;
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