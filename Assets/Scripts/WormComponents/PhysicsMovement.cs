using System.Collections.Generic;
using UnityEngine;

namespace WormComponents
{
    public class PhysicsMovement : MonoBehaviour
    {
        [SerializeField] private float _minGroundNormalY = .65f;
        [SerializeField] private float _gravityModifier = 1f;
        [SerializeField] private Vector2 _velocity;
        [SerializeField] private LayerMask _layerMask;

        protected Vector2 targetVelocity;
        protected bool grounded;
        protected Vector2 groundNormal;
        protected Rigidbody2D _rigidbody2D;
        protected ContactFilter2D contactFilter;
        protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
        protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

        protected const float minMoveDistance = 0.001f;
        protected const float shellRadius = 0.01f;

        void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(_layerMask);
            contactFilter.useLayerMask = true;
        }

        void Update()
        {
            targetVelocity = new Vector2(Input.GetAxis("Horizontal"), 0);

            if (Input.GetKey(KeyCode.Space) && grounded)
                _velocity.y = 5;
        }

        void FixedUpdate()
        {
            _velocity += _gravityModifier * Physics2D.gravity * Time.deltaTime;
            _velocity.x = targetVelocity.x;

            grounded = false;

            Vector2 deltaPosition = _velocity * Time.deltaTime;
            Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
            Vector2 move = moveAlongGround * deltaPosition.x;

            Movement(move, false);

            move = Vector2.up * deltaPosition.y;

            Movement(move, true);
        }

        private void Movement(Vector2 move, bool yMovement)
        {
            float distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                int count = _rigidbody2D.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

                hitBufferList.Clear();

                for (int i = 0; i < count; i++)
                {
                    hitBufferList.Add(hitBuffer[i]);
                }

                for (int i = 0; i < hitBufferList.Count; i++)
                {
                    Vector2 currentNormal = hitBufferList[i].normal;
                    if (currentNormal.y > _minGroundNormalY)
                    {
                        grounded = true;
                        if (yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }

                    float projection = Vector2.Dot(_velocity, currentNormal);
                    if (projection < 0)
                    {
                        _velocity = _velocity - projection * currentNormal;
                    }

                    float modifiedDistance = hitBufferList[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }

            _rigidbody2D.position = _rigidbody2D.position + move.normalized * distance;
        }
    }
}
