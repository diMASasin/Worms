using System;

namespace UltimateCC
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