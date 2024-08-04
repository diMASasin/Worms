using UnityEngine;

namespace CameraFollow
{
    public interface IFollowingCamera
    {
        bool HasTarget { get; }
        void TryZoom(float scrollDeltaY);
        void SetTarget(Transform target);
        void RemoveTarget(Transform target);
        void RemoveAllTargets();
        void MoveToGeneralView();
        void ResetZoom();
    }
}