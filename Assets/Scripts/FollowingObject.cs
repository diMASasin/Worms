using UnityEngine;

public class FollowingObject : MonoBehaviour
{
    [SerializeField] private Transform _followingFor;
    [SerializeField] private Vector2 _offset;

    private void FixedUpdate()
    {
        transform.position = _followingFor.position + (Vector3)_offset;
    }

    public void Connect(Transform target)
    {
        transform.parent = target;
        gameObject.SetActive(false);
    }
    
    public void Connect()
    {
        transform.parent = _followingFor;
        gameObject.SetActive(false);
    }

    public void Disonnect()
    {
        transform.parent = null;
        transform.rotation = Quaternion.identity;
    }
}
