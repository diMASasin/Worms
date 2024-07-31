using TMPro;
using UnityEngine;

namespace _UI.Message
{
    public class MessageView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Animator _animator;

        private static readonly int AppearTrigger = Animator.StringToHash("Appear");
        
        public void Appear(string text)
        {
            _text.text = text;
            _animator.SetTrigger(AppearTrigger);
        }
    }
}