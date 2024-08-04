using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Input_System.InputManager;
using UnityEngine;

namespace WormComponents
{
    public class WormAnimations : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private InputHandler _input;
    
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walk = Animator.StringToHash("Walk");

        private void Update()
        {
            _animator.SetBool(Idle, _input.PlayerData.Physics.IsGrounded);
            _animator.SetBool(Walk, _input.PlayerData.Physics.FacingDirection != 0);
        }
    }
}
