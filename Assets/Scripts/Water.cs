using DG.Tweening;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private float _step;

    public void IncreaseLevel()
    {
        transform.DOMove(transform.position + new Vector3(0, 0 + _step, 0), 1);
    }
}
