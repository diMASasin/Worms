using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingObject : MonoBehaviour
{
    [SerializeField] private Transform _followingFor;
    [SerializeField] private Vector2 _offset;

    private void FixedUpdate()
    {
        transform.position = _followingFor.position + (Vector3)_offset;
    }
}
