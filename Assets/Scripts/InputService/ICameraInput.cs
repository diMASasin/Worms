using Services;

namespace CameraFollow
{
    public interface ICameraInput : IService
    {
        float GetScrollDeltaY();
    }
}