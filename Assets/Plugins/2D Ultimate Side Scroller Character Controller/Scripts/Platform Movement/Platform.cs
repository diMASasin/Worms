using UnityEngine;

namespace UltimateCC
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private Vector2 rightBorderOffset;
        [SerializeField] private Vector2 leftBorderOffset;
        private Vector2 startPoint;
        [SerializeField, Range(-1, 1)] private int direction;
        [SerializeField] private float speed;
        Rigidbody2D rb;

        void Start()
        {
            startPoint = transform.position;
            if (direction == 0)
            {
                direction = 1;
            }
            rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            PlatformMovement();
        }

        private void PlatformMovement()
        {
            if (direction == 1)
            {
                if (transform.position.x < startPoint.x)
                {
                    rb.velocity = new Vector2(direction * speed, 0);
                }
                else if (transform.position.x < startPoint.x + rightBorderOffset.x)
                {
                    rb.velocity = new Vector2(direction * speed, 0);
                }
                else if (transform.position.x >= startPoint.x + rightBorderOffset.x)
                {
                    direction = -1;
                    rb.velocity = new Vector2(direction * speed, 0);
                }
            }
            else if (direction == -1)
            {
                if (transform.position.x > startPoint.x + startPoint.x)
                {
                    rb.velocity = new Vector2(direction * speed, 0);
                }
                else if (transform.position.x > startPoint.x + leftBorderOffset.x)
                {
                    rb.velocity = new Vector2(direction * speed, 0);
                }
                else if (transform.position.x <= startPoint.x + leftBorderOffset.x)
                {
                    direction = 1;
                    rb.velocity = new Vector2(direction * speed, 0);
                }
            }
        }
    }
}
