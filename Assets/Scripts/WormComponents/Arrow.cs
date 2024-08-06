using Timers;
using UnityEngine;
using Zenject;

namespace WormComponents
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private int _lifeTime = 2;
        [SerializeField] private FollowingObject_.FollowingObject _followingObject;
    
        private ITimer _timer;
        
        private static readonly int Move = Animator.StringToHash("Move");

        [Inject]
        public void Construct(ITimer timer) => _timer = timer;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void StartMove(Transform target)
        {
            transform.position = target.position;
            gameObject.SetActive(true);
            
            _followingObject.Follow(target);
            _animator.SetTrigger(Move);

            _timer.Start(_lifeTime, Disable);
        }

        public void Disable()
        {
            _timer.Stop();
            gameObject.SetActive(false);
        }
    }
}
