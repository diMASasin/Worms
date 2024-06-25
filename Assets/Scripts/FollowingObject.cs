using System;
using UnityEngine;
using Zenject;

public class FollowingObject : MonoBehaviour, ILateTickable
{
    [field: SerializeField] public FollowingObjectConfig Config { get; private set; }

    private Vector3 _newPosition;
    private FollowingObjectConfig _followingObjectConfig;
    public Vector3 MoveTo { get; private set; }
    public Transform FollowingFor { get; private set; }
    public bool FreezeZPosition { get; private set; }

    public Vector3 CurrentPosition
    {
        get => transform.position;
        protected set => transform.position = value;
    }

    public void LateTick()
    {
        _newPosition = FollowingFor != null ? FollowingFor.position + _followingObjectConfig.Offset : MoveTo;

        if (FreezeZPosition == true && FollowingFor != null)
            _newPosition.z = transform.position.z;

        Move(_newPosition);
    }

    protected void Move(Vector3 newPosition)
    {
        if (Config.MoveSmoothly == true)
            CurrentPosition = Vector3.Lerp(CurrentPosition, newPosition, Config.Speed * Time.deltaTime);
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