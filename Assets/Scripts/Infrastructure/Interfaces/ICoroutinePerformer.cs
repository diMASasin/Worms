using System.Collections;
using UnityEngine;

namespace Infrastructure
{
    public interface ICoroutinePerformer
    {
        public Coroutine StartCoroutine(IEnumerator enumerator);
        public void StopCoroutine(Coroutine coroutine);
        public void StopCoroutine(IEnumerator coroutine);
    }
}