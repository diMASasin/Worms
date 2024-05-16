using System;
using EventProviders;
using Projectiles;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private int _maxPosition = 60;
    
    [field: SerializeField] public int MinPosition { get; private set; } = 10;

    private Transform _target;

    private bool _shouldZoomToTarget;

    private Func<Vector3> _getTargetPosition;

    private Vector3 CameraPosition
    {
        get => _camera.transform.position;
        set => _camera.transform.position = value;
    }

    private void Update()
    {
        TryZoom();
        
        if(_shouldZoomToTarget == true)
            ZoomToTarget();
    }

    private void FixedUpdate()
    {
        if (_target != null)
            FollowTarget();
    }

    public void ZoomTarget()
    {
        _shouldZoomToTarget = true;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _getTargetPosition = () => _target.position + _offset;
    }

    public void SetTarget(Vector3 positionTarget)
    {
        _getTargetPosition = () => positionTarget;
    }

    private void TryZoom()
    {
        if (Input.mouseScrollDelta.y < 0 && CameraPosition.z > MinPosition ||
            Input.mouseScrollDelta.y > 0 && CameraPosition.z < _maxPosition)
            CameraPosition += new Vector3(0, 0, Input.mouseScrollDelta.y);

        float newPositionZ = Mathf.Clamp(CameraPosition.z, MinPosition, _maxPosition);

        CameraPosition = new Vector3(CameraPosition.x, CameraPosition.y, newPositionZ);
    }

    private void ZoomToTarget()
    {
        float tolerance = 0.2f;
        float positionZ = transform.position.z - _offset.z;
        float targetPositionZ = _getTargetPosition().z;
        
        if (Math.Abs(positionZ - targetPositionZ) < 2 ||
            Math.Abs(positionZ - _maxPosition) < tolerance || 
            Math.Abs(positionZ - MinPosition) < tolerance)
            _shouldZoomToTarget = false;
    }

    private void FollowTarget()
    {
        Vector3 newPosition = _getTargetPosition();
        
        if(_shouldZoomToTarget == false)
            newPosition.z = transform.position.z;

        CameraPosition = Vector3.Lerp(CameraPosition, newPosition, _speed * Time.deltaTime);
    }
}