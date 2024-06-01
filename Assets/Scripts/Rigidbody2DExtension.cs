using UnityEngine;

public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D rigidbody, float explosionForce, Vector2 explosionPosition,
        float colliderRadius, float upwardsModifier = 0.0f, ForceMode2D mode = ForceMode2D.Impulse)
    {
        Vector2 explosionDirection = rigidbody.position - explosionPosition;
        float explosionDistance = explosionDirection.magnitude;

        // Normalize without computing magnitude again
        if (upwardsModifier == 0)
        {
            explosionDirection /= explosionDistance;
        }
        else
        {
            // From Rigidbody.AddExplosionForce doc:
            // If you pass a non-zero value for the upwardsModifier parameter, the direction
            // will be modified by subtracting that value from the Y component of the centre point.
            explosionDirection.y += upwardsModifier;
            explosionDirection.Normalize();
        }

        float interpolationValue = Mathf.Clamp(colliderRadius - explosionDistance, 0, colliderRadius);
        float newExplosionForce = Mathf.Lerp(0, explosionForce, interpolationValue / colliderRadius);
        rigidbody.AddForce(newExplosionForce * explosionDirection, mode);
    }
}
