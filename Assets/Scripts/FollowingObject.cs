using System;
using UnityEngine;

[Serializable]
public class FollowingObject : IFixedTickable
{
    [SerializeField] private Transform _objectTransform;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _speed = 1000;

    public Transform FollowingFor { get; private set; }
    public bool FreezeZPosition { get; private set; }
    private Vector3 _newPosition;
    private Vector3 _moveTo;

    public void LateTick()
    {
        _newPosition = FollowingFor != null ? FollowingFor.position : _moveTo;
        
        if (FreezeZPosition == true)
            _newPosition.z = _objectTransform.position.z - _offset.z;
        
        _objectTransform.position = Vector3.Lerp(_objectTransform.position, _newPosition + _offset, 
            _speed * Time.deltaTime);
    }

    public void Follow(Transform target)
    {
        if(target == null)
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
