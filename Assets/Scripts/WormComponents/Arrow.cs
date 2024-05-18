using Timers;
using UnityEngine;

namespace WormComponents
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private int _lifeTime = 2;
    
        private static readonly int Move = Animator.StringToHash("Move");
        private readonly Timer _timer = new();

        public void Tick() => _timer.Tick();
    
        public void StartMove(Transform parentTransform)
        {
            transform.parent = parentTransform; 
            transform.position = parentTransform.position;
        
            gameObject.SetActive(true);
            _animator.SetTrigger(Move);

            _timer.Start(_lifeTime, Disable);
        }

        private void Disable()
        {
            gameObject.transform.parent = null;
            gameObject.SetActive(false);
        }
    }
}
