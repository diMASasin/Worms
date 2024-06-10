using System.Collections;
using Services;
using UnityEngine;

namespace Infrastructure
{
    public interface ICoroutinePerformer : IService
    {
        public Coroutine StartCoroutine(IEnumerator enumerator);
        public void StopCoroutine(Coroutine coroutine);
        public void StopCoroutine(IEnumerator coroutine);
    }
}