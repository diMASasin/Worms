using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutinePerformer : MonoBehaviour
{
    private static CoroutinePerformer _instance;

    public void Init()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(_instance);
    }

    public static void StartRoutine(IEnumerator enumerator)
    {
        _instance.StartCoroutine(enumerator);
    }
}