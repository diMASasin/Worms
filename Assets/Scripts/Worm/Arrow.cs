using Timers;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private int _lifeTime = 2;

    public void StartMove()
    {
        gameObject.SetActive(true);
        _animator.SetTrigger("Move");

        Timer timer = new Timer();
        timer.Start(_lifeTime, Disable);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
