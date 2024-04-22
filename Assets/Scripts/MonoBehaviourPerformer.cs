using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourPerformer : MonoBehaviour
{
    private static MonoBehaviourPerformer _instance;

    private static List<IFixedTickable> _tickables = new();

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(_instance);
    }

    private void FixedUpdate()
    {
        foreach (var tickable in _tickables)
            tickable.FixedTick();
    }

    public static void StartRoutine(IEnumerator enumerator)
    {
        _instance.StartCoroutine(enumerator);
    }

    public static void AddFixedTickable(IFixedTickable tickable)
    {
        _tickables.Add(tickable);
    }

}