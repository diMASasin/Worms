using UI;
using UnityEngine.Pool;
using static UnityEngine.Object;

namespace Pools
{
    public class FollowingTimerViewPool : IFollowingTimerViewPool
    {
        private readonly ObjectPool<FollowingTimerView> _followingTimerViewPool;
        private FollowingTimerView _followingTimerViewPrefab;

        public FollowingTimerViewPool(FollowingTimerView followingTimerViewPrefab)
        {
            _followingTimerViewPrefab = followingTimerViewPrefab;
            
            _followingTimerViewPool = new ObjectPool<FollowingTimerView>(
                () => Instantiate(_followingTimerViewPrefab),
                timer => timer.gameObject.SetActive(true),
                timer => timer.gameObject.SetActive(false));
        }

        public FollowingTimerView Get() => _followingTimerViewPool.Get();
        
        public void Release(FollowingTimerView timerView) => _followingTimerViewPool.Release(timerView);
    }
}