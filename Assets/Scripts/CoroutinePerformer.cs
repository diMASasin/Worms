using System.Collections;
using UnityEngine;

public class CoroutinePerformer : MonoBehaviour
{
    private static CoroutinePerformer _instance;
    private static MonoBehaviour _instanceMono;

    public void Init()
    {
        if (_instance == null)
        {
            _instance = this;
            _instanceMono = _instance;
        }
        else
        {
            Destroy(_instance);
        }
    }

    public new static Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return _instanceMono.StartCoroutine(enumerator);
    }
    
    public new static void StopCoroutine(IEnumerator enumerator)
    {
        _instanceMono.StopCoroutine(enumerator);
    }
}