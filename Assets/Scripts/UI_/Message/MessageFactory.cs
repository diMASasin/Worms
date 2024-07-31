using System;
using UnityEngine;
using static UnityEngine.Object;

namespace _UI.Message
{
    [Serializable]
    public class MessageFactory
    {
        [SerializeField] private MessageView _messageViewPrefab;
        [SerializeField] private Transform _parent;
        
        public MessageView Create()
        {
            return Instantiate(_messageViewPrefab, _parent);
        }
    }
}