using Infrastructure;
using Timers;
using UnityEngine;

namespace WormComponents
{
    public class Arrow : MonoBehaviour, ICoroutinePerformer
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private int _lifeTime = 2;
        [SerializeField] private FollowingObject _followingObject;
    
        private readonly Timer _timer;
        
        private static readonly int Move = Animator.StringToHash("Move");

        public Arrow() => _timer = new Timer(this);

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
