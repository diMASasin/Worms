using UI;
using UnityEngine.Pool;
using static UnityEngine.Object;

namespace Pools
{
    public class FollowingTimerViewPool : IPool<FollowingTimerView>
    {
        private readonly ObjectPool<FollowingTimerView> _followingTimerViewPool;

        public FollowingTimerViewPool(FollowingTimerView followingTimerViewPrefab)
        {
            _followingTimerViewPool = new ObjectPool<FollowingTimerView>(
                () => Instantiate(followingTimerViewPrefab),
                timer => timer.gameObject.SetActive(true),
                timer => timer.gameObject.SetActive(false));
        }

        public FollowingTimerView Get() => _followingTimerViewPool.Get();
        
        public void Release(FollowingTimerView timerView) => _followingTimerViewPool.Release(timerView);
    }
}