using UnityEngine;

public class SheepMovement : Movement
{
    private Vector3 _overlapPoint;
    private Vector3 _overlapBoxSize;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        _overlapPoint = transform.position +
            new Vector3(-_collider.bounds.size.x / 2 * _armature.transform.right.x, 0, 0) -
            new Vector3(0.1f * _armature.transform.right.x, 0, 0);
        _overlapBoxSize = new Vector2(0.01f, 0.15f);

        var overlap = Physics2D.OverlapBox(_overlapPoint, _overlapBoxSize, 0, _layerMask);

        if (overlap != null && _groundChecker.IsGrounded)
        {
            Horizontal = -Horizontal;
            ResetVelocity();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(_overlapPoint, _overlapBoxSize);
    }
}
