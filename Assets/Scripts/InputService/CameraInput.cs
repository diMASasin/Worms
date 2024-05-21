using CameraFollow;
using UnityEngine;

namespace InputService
{
    public class CameraInput
    {
        private readonly IControllableCamera _camera;

        public CameraInput(IControllableCamera camera)
        {
            _camera = camera;
        }
        
        public void Tick()
        {
            _camera.TryZoom(Input.mouseScrollDelta.y);
        }
    }
}