using UnityEngine;

namespace UltimateCC
{
    public static class Vector2Extensions
    {
        // Rotates a 2D vector around the origin (0, 0) by a specified angle in degrees.
        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float x = vector.x * cos - vector.y * sin;
            float y = vector.x * sin + vector.y * cos;

            return new Vector2(x, y);
        }

        public static Vector2 RotateAround(this Vector2 point, Vector2 pivot, float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            // Translate the point to the origin (subtract pivot)
            point -= pivot;

            // Rotate the point
            float xNew = point.x * cos - point.y * sin;
            float yNew = point.x * sin + point.y * cos;

            // Translate the point back to the original position (add pivot)
            return new Vector2(xNew + pivot.x, yNew + pivot.y);
        }
    }
}