using System;
using UnityEngine;

[Serializable]
public class FollowingObject : IFixedTickable
{
    [SerializeField] private Transform _objectTransform;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _speed = 1000;
    
    private Transform _followingFor;
    private Vector3 _newPosition;
    private Func<Vector3> _getTargetPosition;
    private bool _freezeZPosition;

    public Vector3 TargetPosition => _getTargetPosition();

    public void LateTick()
    {
        Vector3 newPosition = _getTargetPosition();
        
        if (_freezeZPosition == true)
            newPosition.z = _objectTransform.position.z - _offset.z;
        
        _objectTransform.position = Vector3.Lerp(_objectTransform.position, newPosition + _offset, 
            _speed * Time.deltaTime);
    }

    public void Follow(Transform target)
    {
        if(target == null)
            return;
        
        _getTargetPosition = () =>
        {
            if (target == null)
                return _objectTransform.position;
            else
                return target.position;
        };
    }
    
    public void Follow(Func<Vector3> getTargetPosition)
    {
        _getTargetPosition = getTargetPosition;
    }

    public void StopFollowZPosition()
    {
        _freezeZPosition = true;
    }

    public void StopFollow()
    {
        _followingFor = null;
    }
}
