using UnityEngine;

public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D rigidbody, float explosionForce, Vector2 explosionPosition,
        float explosionRadius, float upwardsModifier = 0.0f, ForceMode2D mode = ForceMode2D.Impulse)
    {
        Vector2 explosionDirection = rigidbody.position - explosionPosition;
        float explosionDistance = explosionDirection.magnitude;

        if (upwardsModifier == 0)
        {
            explosionDirection /= explosionDistance;
        }
        else
        {
            explosionDirection.y += upwardsModifier;
            explosionDirection.Normalize();
        }

        float interpolationValue = Mathf.Clamp(explosionRadius - explosionDistance, 0, explosionRadius);
            
        float minForce = explosionForce * 0.5f;
        explosionForce = interpolationValue >= explosionForce * 0.9f ? explosionForce : interpolationValue;
        
        float newExplosionForce = Mathf.Lerp(minForce, explosionForce, interpolationValue / explosionRadius);
        rigidbody.AddForce(newExplosionForce * explosionDirection, mode);
    }
}
