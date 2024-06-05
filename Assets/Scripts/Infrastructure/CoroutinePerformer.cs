using System.Collections;
using UnityEngine;

namespace Infrastructure
{
    public class CoroutinePerformer : MonoBehaviour, ICoroutinePerformer
    {
        public new Coroutine StartCoroutine(IEnumerator enumerator) => 
            ((MonoBehaviour)this).StartCoroutine(enumerator);
        public new void StopCoroutine(Coroutine coroutine) => 
            ((MonoBehaviour)this).StopCoroutine(coroutine);
    }
}