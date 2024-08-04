using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Essentials.Custom_Attributes;
using UnityEngine;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Platform_Movement
{
    public class SwingingPlatform : MonoBehaviour
    {
        [SerializeField] private float maxAngle;
        [SerializeField, NonEditable] private float direction;
        [SerializeField] private float speed;
        Rigidbody2D rb;

        void Start()
        {
            direction = 1;
            rb = transform.GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            PlatformMovement();
        }

        private void PlatformMovement()
        {
            rb.angularVelocity = direction * speed;

            float _offset = 0.5f;
            if ((rb.rotation > maxAngle - _offset && direction == 1) || (rb.rotation < -maxAngle + _offset && direction == -1))
            {
                direction *= -1;
            }
        }
    }
}
