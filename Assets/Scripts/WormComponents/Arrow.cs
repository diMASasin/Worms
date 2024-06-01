using Timers;
using UnityEngine;

namespace WormComponents
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private int _lifeTime = 2;
        [SerializeField] private FollowingObject _followingObject;
    
        private readonly Timer _timer = new();
        
        private static readonly int Move = Animator.StringToHash("Move");

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void LateUpdate() => _followingObject.LateTick();

        public void StartMove(Transform target)
        {
            transform.position = target.position;
            gameObject.SetActive(true);
            
            _followingObject.Follow(target);
            _animator.SetTrigger(Move);

            _timer.Start(_lifeTime, Disable);
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
