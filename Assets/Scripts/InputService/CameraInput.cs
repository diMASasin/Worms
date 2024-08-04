using UnityEngine;

namespace InputService
{
    public class CameraInput : ICameraInput
    {
        public float GetScrollDeltaY()
        {
            return Input.mouseScrollDelta.y;
        }
    }
}