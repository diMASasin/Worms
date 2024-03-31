using UnityEngine;

public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Impulse)
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;

        // Normalize without computing magnitude again
        if (upwardsModifier == 0)
            explosionDir /= explosionDistance;
        else
        {
            // From Rigidbody.AddExplosionForce doc:
            // If you pass a non-zero value for the upwardsModifier parameter, the direction
            // will be modified by subtracting that value from the Y component of the centre point.
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

        rb.AddForce(Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
    }
}
