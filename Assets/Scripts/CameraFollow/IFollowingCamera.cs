using UnityEngine;

namespace CameraFollow
{
    public interface IFollowingCamera
    {
        void TryZoom(float scrollDeltaY);
        void SetTarget(Transform target);
        void RemoveTarget(Transform target);
        void RemoveAllTargets();
        void MoveToGeneralView();
        void ResetZoom();
    }
}