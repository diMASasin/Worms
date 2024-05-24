using UI;

namespace Pools
{
    public interface IFollowingTimerViewPool
    {
        FollowingTimerView Get();
        void Release(FollowingTimerView timerView);
    }
}