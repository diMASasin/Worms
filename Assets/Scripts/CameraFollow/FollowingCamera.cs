using System;
using InputService;
using UnityEngine;

namespace CameraFollow
{
    [RequireComponent(typeof(Camera))]
    public class FollowingCamera : MonoBehaviour, IControllableCamera
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _speed = 1;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private int _maxPosition = 60;
        [SerializeField] private FollowingObject _followingObject;
        [field: SerializeField] public int MinPosition { get; private set; } = 10;
        [field: SerializeField] public Vector3 GeneralViewPosition { get; private set; }

        private Transform _target;

        private Vector3 CameraPosition
        {
            get => _camera.transform.position;
            set => _camera.transform.position = value;
        }

        private void Update()
        {
                StopZoom();
        }

        private void LateUpdate()
        {
            _followingObject.LateTick();
        }

        public void SetTarget(Transform target)
        {
            _followingObject.Follow(() =>
            {
                if (target == null)
                    return transform.position;
                else
                    return target.position;
            });
        }

        public void SetTarget(Vector3 positionTarget)
        {
            _followingObject.Follow(() => positionTarget);
        }

        public void TryZoom(float scrollDeltaY)
        {
            if (scrollDeltaY < 0 && CameraPosition.z > MinPosition ||
                scrollDeltaY > 0 && CameraPosition.z < _maxPosition)
                CameraPosition += new Vector3(0, 0, scrollDeltaY);

            float newPositionZ = Mathf.Clamp(CameraPosition.z, MinPosition, _maxPosition);

            CameraPosition = new Vector3(CameraPosition.x, CameraPosition.y, newPositionZ);
        }
        
        private void StopZoom()
        {
            float tolerance = 0.2f;
            float positionZ = transform.position.z - _offset.z;
            float targetPositionZ = _followingObject.TargetPosition.z;
        
            if (Math.Abs(positionZ - targetPositionZ) < 2 ||
                Math.Abs(positionZ - _maxPosition) < tolerance || 
                Math.Abs(positionZ - MinPosition) < tolerance)
                _followingObject.StopFollowZPosition();
        }
    }
}