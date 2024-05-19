using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pools
{
    public class ObjectPool2<T> where T : Object
    {
        private readonly T _prefab;
        private readonly Transform _objectsParent;
        private readonly int _amount;

        private readonly List<T> _objects = new();
        private readonly List<T> _used = new();

        public event Action<T> Got;
        public event Action<T> Removed;
        public event Action<T> Created;

        public static int Count { get; private set; }

        public IReadOnlyList<T> Objects => _objects;

        public ObjectPool2(T prefab, Transform objectsParent, int amount)
        {
            _prefab = prefab;
            _objectsParent = objectsParent;
            _amount = amount;
        }

        public void CreateObjects()
        {
            for (int i = 0; i < _amount; i++)
                CreateObject();
        }

        public T Get()
        {
            if (_used.Count == _objects.Count)
                CreateObject();

            var freeObject = _objects.First(obj => !_used.Contains(obj));
            Got?.Invoke(freeObject);

            _used.Add(freeObject);
            Count++;
            return freeObject;
        }

        public virtual void Remove(T obj)
        {
            _used.Remove(obj);
            Removed?.Invoke(obj);
            Count--;
        }

        protected virtual T CreateObject()
        {
            var obj = Object.Instantiate(_prefab, _objectsParent);
            _objects.Add(obj);
            Created?.Invoke(obj);
            return obj;
        }
    }
}