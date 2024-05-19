using CameraFollow;
using UnityEngine;

namespace InputService
{
    public class CameraInput
    {
        private readonly FollowingCamera _camera;

        public CameraInput(FollowingCamera camera)
        {
            _camera = camera;
        }
        
        public void Tick()
        {
            _camera.TryZoom(Input.mouseScrollDelta.y);
        }
    }
}