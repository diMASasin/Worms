using Timers;
using UnityEngine;

namespace MovementComponents
{
    public class SheepMovement : Movement
    {
        [SerializeField] private float _jumpInterval = 2;

        private Timer _jumpTimer;
        
        private Vector3 _overlapPoint;
        private Vector3 _overlapBoxSize;
        private bool _shouldJump;

        protected override void Start()
        {
            base.Start();
            _jumpTimer = new Timer();
        }
        
        public override void Reset()
        {
            base.Reset();
            _shouldJump = false;
            _jumpTimer.Stop();
        }
        
        private void OnEnable()
        {
            MoveDircetionChanged += TryJumpAfterDelay;
            GroundChecker.IsGroundedChanged += OnIsGroundedChanged;
        }

        private void OnDisable()
        {
            MoveDircetionChanged -= TryJumpAfterDelay;
            GroundChecker.IsGroundedChanged -= OnIsGroundedChanged;
            
            Reset();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            var right = -Armature.transform.right;

            _overlapPoint = Armature.transform.position +
                            new Vector3(-Collider.bounds.size.x / 1.9f * right.x, 0, 0) -
                            new Vector3(0.1f * right.x, 0, 0);
            _overlapBoxSize = new Vector2(0.01f, 0.1f);

            var overlap = Physics2D.OverlapBox(_overlapPoint, _overlapBoxSize, 0, LayerMask);

            if (overlap != null && GroundChecker.IsGrounded) 
                Horizontal = -Horizontal;
        }

        private void OnIsGroundedChanged(bool isGrounded)
        {
            // Debug.Log($"OnIsGroundedChanged");
            if(isGrounded == true && _shouldJump == true)
                RepeatLongJump();
        }

        private void TryJumpAfterDelay()
        {
            Debug.Log($"TryJumpAfterDelay");
            _jumpTimer.Start(_jumpInterval, () =>
            {
                if(GroundChecker.IsGrounded == true)
                    RepeatLongJump();
                else
                {
                    Debug.Log($"Should jump");
                    _shouldJump = true;
                }
            });
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawCube(_overlapPoint, _overlapBoxSize);
        }

        private void RepeatLongJump()
        {
            Debug.Log($"Jump");
            LongJump();
            _jumpTimer.Start(_jumpInterval, TryJumpAfterDelay);
        }
    }
}
