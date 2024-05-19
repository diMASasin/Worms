using System.Runtime.InteropServices;
using UnityEngine;

namespace ScriptBoy.Digable2DTerrain.Scripts
{
    public class Triangulator
    {
#if UNITY_IPHONE
		[DllImport("__Internal"")]
#else
        [DllImport("Triangulator")]
#endif
        private static extern void Triangulate(int length, float[] X, float[] Y, int[] triangles);

        public static int[] Triangulate_CPP(Vector2[] polygon)
        {
            int length = polygon.Length;
            if (length < 2) return null;

            float[] X = new float[length];
            float[] Y = new float[length];

            for (int i = 0; i < length; i++)
            {
                X[i] = polygon[i].x;
                Y[i] = polygon[i].y;
            }

            int[] triangles = new int[(length - 2) * 3];
            Triangulate(length, X, Y, triangles);

            return triangles;
        }

        public static int[] Triangulate_CSharp(Vector2[] polygon)
        {
            return Dll.D2DT.Triangulator.Triangulate(polygon);
        }
    }
}