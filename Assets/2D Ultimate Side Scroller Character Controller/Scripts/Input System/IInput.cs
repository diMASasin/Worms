using System;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Input_System
{
    public interface IMovementInput
    {
        void Enable();
        void Disable();
        public event Action<bool> LongJumpStarted;
        public event Action<bool> HighJumpStarted;
        public event Action<float> WalkPerformed;
        public event Action<bool> DashPerformed;
        public event Action<bool> CrouchPerformed;
        public event Action<bool> WallGrabPerformed;
        public event Action<float> WallClimbPerformed;
    }
}