using System;
using InputService;
using UnityEngine;
using Zenject;

namespace CameraFollow
{
    [RequireComponent(typeof(Camera))]
    public class FollowingCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [field: SerializeField] public int MinPosition { get; private set; } = -65;
        [field: SerializeField]public int MaxPosition { get; set; } = -10;
        [field: SerializeField] public Vector3 GeneralViewPosition { get; private set; }
        [SerializeField] private FollowingObject _followingObject;

        private Transform _target;
        private ICameraInput _cameraInput;

        private Vector3 CameraPosition
        {
            get => _camera.transform.position;
            set => _camera.transform.position = value;
        }

        [Inject]
        public void Construct(ICameraInput cameraInput)
        {
            _cameraInput = cameraInput;
        }

        private void Update()
        {
            if (_cameraInput == null) return;
            
            TryZoom(_cameraInput.GetScrollDeltaY());
            TryStopZoom();
        }

        public void SetTarget(Transform target)
        {
            _followingObject.Follow(target);
        }

        public void SetTarget(Vector3 positionTarget)
        {
            _followingObject.StopFollow();
            _followingObject.Follow(positionTarget);
        }

        public void TryZoom(float scrollDeltaY)
        {
            if (scrollDeltaY < 0 && CameraPosition.z > MinPosition ||
                scrollDeltaY > 0 && CameraPosition.z < MaxPosition)
                CameraPosition += new Vector3(0, 0, scrollDeltaY);

            float newPositionZ = Mathf.Clamp(CameraPosition.z, MinPosition, MaxPosition);

            CameraPosition = new Vector3(CameraPosition.x, CameraPosition.y, newPositionZ);
        }
        
        private void TryStopZoom()
        {
            if(_followingObject.FreezeZPosition == true)
                return;

            float targetPositionZ;
            if (_followingObject.FollowingFor == null)
                targetPositionZ = _followingObject.MoveTo.z;
            else
                targetPositionZ = _followingObject.FollowingFor.position.z;
            
            float tolerance = 0.2f;
            float positionZ = transform.position.z - _followingObject.Config.Offset.z;

            if (Math.Abs(positionZ - targetPositionZ) < 2 ||
                Math.Abs(positionZ - MaxPosition) < tolerance ||
                Math.Abs(positionZ - MinPosition) < tolerance)
            {
                _followingObject.StopFollowZPosition();
            }
        } 
    }
}