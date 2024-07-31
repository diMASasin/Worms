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

        float interpolationValue = Mathf.Clamp(explosionDistance / explosionRadius, 0, explosionRadius);
            
        float minForce = explosionForce * 0.5f;
        explosionForce = interpolationValue >= explosionForce * 0.9f ? explosionForce : interpolationValue;
        
        float newExplosionForce = Mathf.Lerp(minForce, explosionForce, interpolationValue);
        rigidbody.AddForce(newExplosionForce * explosionDirection, mode);
    }

    public static void AddExplosionForce(this Rigidbody2D rigidbody, Vector3 direction, float explosionForce,
        float forceMultiplier, float upwardsModifier = 0.0f, ForceMode2D mode = ForceMode2D.Impulse)
    {
        direction.y = Mathf.Clamp(direction.y, 0.1f, 10f);
        
        direction.y *= upwardsModifier;
        direction.Normalize();

        float clampedMultiplier = Mathf.Clamp(forceMultiplier, 0.3f, 1);
        Vector3 newExplosionForce = direction * explosionForce * clampedMultiplier;
        
        Debug.Log($"clampedMultiplier: {clampedMultiplier}");
        Debug.Log($"newExplosionForce: {newExplosionForce}");
        rigidbody.AddForce(newExplosionForce, mode);
    }
}
