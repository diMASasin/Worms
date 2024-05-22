using UnityEngine;

public class FollowingObject : MonoBehaviour
{
    [SerializeField] private Transform _followingFor;
    [SerializeField] private Vector2 _offset;

    private void FixedUpdate()
    {
        if(_followingFor != null)
            transform.position = _followingFor.position + (Vector3)_offset;
    }

    public void Connect(Transform target)
    {
        _followingFor = target;
    }

    public void Disonnect()
    {
        _followingFor = null;
    }
}
