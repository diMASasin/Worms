using System.Collections;
using UnityEngine;

namespace Infrastructure
{
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
        }

        public new Coroutine StartRoutine(IEnumerator enumerator)
        {
            return _instanceMono.StartCoroutine(enumerator);
        }
    
        public new static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return _instanceMono.StartCoroutine(enumerator);
        }
    
        public new static void StopCoroutine(Coroutine coroutine)
        {
            _instanceMono.StopCoroutine(coroutine);
        }
    }
}