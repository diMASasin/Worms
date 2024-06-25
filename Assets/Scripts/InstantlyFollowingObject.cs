using UnityEngine;

public class InstantlyFollowingObject : FollowingObject
{
    protected void Move(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}