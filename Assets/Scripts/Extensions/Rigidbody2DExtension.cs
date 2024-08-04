using UnityEngine;

namespace Extensions
{
    public static class Rigidbody2DExtension
    {
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
}
