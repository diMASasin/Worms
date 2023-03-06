using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private int _lifeTime = 2;

    public void StartMove()
    {
        gameObject.SetActive(true);
        _animator.SetTrigger("Move");
        Invoke(nameof(Disable), _lifeTime);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
