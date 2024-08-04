using System;
using System.Linq;
using Cinemachine;
using InputService;
using UnityEngine;
using Zenject;
using static Cinemachine.CinemachineTargetGroup;

namespace CameraFollow
{
    public class CinemachineFollowingCamera : MonoBehaviour, IFollowingCamera
    {
        [SerializeField] private CinemachineTargetGroup _targetGroup;
        [SerializeField] private CinemachineVirtualCamera _mainCamera;
        [SerializeField] private CinemachineVirtualCamera _generalViewCamera;
        [SerializeField] private CinemachineCameraOffset _cinemachineCameraOffset;
        
        [SerializeField] private float _minPosition;
        [SerializeField] private float _maxPosition;
        
        private ICameraInput _cameraInput;

        public bool HasTarget => _targetGroup.m_Targets.Length > 0;

        private float CameraZOffset
        {
            get => _cinemachineCameraOffset.m_Offset.z;
            set => _cinemachineCameraOffset.m_Offset.z = value;
        }

        [Inject]
        private void Construct(ICameraInput cameraInput)
        {
            _cameraInput = cameraInput;
            
            _mainCamera.enabled = false;
            _generalViewCamera.enabled = true;
        }

        private void Update() => TryZoom(_cameraInput.GetScrollDeltaY());


        public void ResetZoom()
        {
            _cinemachineCameraOffset.m_Offset = Vector3.zero;
        }

        public void TryZoom(float scrollDeltaY)
        {
            float newPositionZ = CameraZOffset;
            
            if (scrollDeltaY < 0 && CameraZOffset + scrollDeltaY > _minPosition ||
                scrollDeltaY > 0 && CameraZOffset + scrollDeltaY < _maxPosition)
                newPositionZ += scrollDeltaY;

            newPositionZ = Mathf.Clamp(newPositionZ, _minPosition, _maxPosition);

            CameraZOffset = newPositionZ;
        }

        public void RemoveTarget(Transform target)
        {
            if(_targetGroup.m_Targets.Count(t => t.target == target) == 0)
                return;
            
            _targetGroup.RemoveMember(target);
        }

        public void RemoveAllTargets()
        {
            _targetGroup.m_Targets = Array.Empty<Target>();
        }

        public void MoveToGeneralView()
        {
            _generalViewCamera.enabled = true;
            _mainCamera.enabled = false;
        }

        public void SetTarget(Transform target)
        {
            if(_targetGroup.m_Targets.Count(t => t.target == target) > 0)
                return;

            _mainCamera.enabled = true;
            _generalViewCamera.enabled = false;
            _targetGroup.AddMember(target, 10f, 1);
        }
    }
}