using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.Events;

namespace WormComponents
{
    [Serializable]
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider;
        [SerializeField] private GroundCheckerConfig _config;
        
        private List<Collider2D> _contacts = new();

        public bool IsGrounded { get; private set; }

        public event Action<bool> IsGroundedChanged;

        public void FixedUpdate()
        {
            Physics2D.OverlapBox(GetPoint(), _config.Size, 0, _config.ContactFilter2D, _contacts);

            if(_contacts.Contains(_collider))
                _contacts.Remove(_collider);
            IsGrounded = _contacts.Count > 0;

            IsGroundedChanged?.Invoke(IsGrounded);
        }

        public void OnDrawGizmos()
        {
            if(_config.ShowGroundCheckerBox)
                Gizmos.DrawCube(GetPoint(), _config.Size);
        }

        private Vector2 GetPoint()
        {
            return (Vector2)transform.position + _config.Offset;
        }
    }
}
