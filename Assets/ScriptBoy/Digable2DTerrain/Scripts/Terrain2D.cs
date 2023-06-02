using UnityEngine;
namespace ScriptBoy.Digable2DTerrain
{
    public class Terrain2D : Dll.D2DT.Terrain2D
    {
        public override int[] TriangulatePolygon(Vector2[] polygon)
        {
           return Triangulator.Triangulate_CPP(polygon);//Triangulate with Native plug-in
           //return Triangulator.Triangulate_CSharp(polygon);//Triangulate with Managed plug-in
        }
    }
}
