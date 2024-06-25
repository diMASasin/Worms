using UnityEngine;

public class SmoothlyFollowingObject : FollowingObject
{
    protected void Move(Vector3 newPosition)
    {
        CurrentPosition = Vector3.Lerp(CurrentPosition, newPosition, Config.Speed * Time.deltaTime);         
    }
}