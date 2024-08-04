using UnityEngine;

namespace FollowingObject
{
    public class FollowingObject : MonoBehaviour
    {
        [field: SerializeField] public bool MoveSmoothly { get; private set; }
        [field: SerializeField] public float Speed { get; private set; } = 10;
        [field: SerializeField] public Vector3 Offset { get; private set; }

        private Vector3 _newPosition;
        public Vector3 MoveTo { get; private set; }
        public Transform FollowingFor { get; private set; }
        public bool FreezeZPosition { get; private set; }

        public Vector3 CurrentPosition
        {
            get => transform.position;
            protected set => transform.position = value;
        }

        public void LateUpdate()
        {
            _newPosition = FollowingFor != null ? FollowingFor.position + Offset : MoveTo;

            if (FreezeZPosition == true && FollowingFor != null)
                _newPosition.z = transform.position.z;

            Move(_newPosition);
        }

        protected void Move(Vector3 newPosition)
        {
            if (MoveSmoothly == true)
                CurrentPosition = Vector3.Lerp(CurrentPosition, newPosition, Speed * Time.deltaTime);
            else
                transform.position = newPosition;
        }

        public void Follow(Transform target)
        {
            if (target == null)
                return;

            FreezeZPosition = false;
            FollowingFor = target;
        }

        public void Follow(Vector3 newPosition)
        {
            FreezeZPosition = false;
            MoveTo = newPosition;
        }

        public void StopFollowZPosition()
        {
            FreezeZPosition = true;
        }

        public void StopFollow()
        {
            FollowingFor = null;
        }
    }
}