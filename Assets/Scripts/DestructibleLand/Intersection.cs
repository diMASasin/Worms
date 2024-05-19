using UnityEngine;

namespace DestructibleLand
{
	public class Intersection
	{

		public static bool IsIntersecting(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
		{
			bool isIntersecting = false;
			float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
			// Если denominator == 0 значит линии параллельны
			if (denominator != 0)
			{
				float u_a = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
				float u_b = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;
				//Is intersecting if u_a and u_b are between 0 and 1
				if (u_a >= 0 && u_a <= 1 && u_b >= 0 && u_b <= 1)
				{
					isIntersecting = true;
				}
			}
			return isIntersecting;
		}


		public static Vector2 GetIntersection(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
		{
			float top = (D.x - C.x) * (A.y - C.y) - (D.y - C.y) * (A.x - C.x);
			float bottom = (D.y - C.y) * (B.x - A.x) - (D.x - C.x) * (B.y - A.y);
			float t = top / bottom;
			Vector2 result = Vector2.Lerp(A,B, t);
			return result;
		}


	}
}


