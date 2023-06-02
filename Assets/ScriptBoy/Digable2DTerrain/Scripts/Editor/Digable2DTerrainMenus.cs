using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ScriptBoy.Digable2DTerrain
{
    public class Menus
    {
        [MenuItem("Tools/ScriptBoy/Digable2DTerrain/Terrain2D", false, 0)]
        public static void AddGround2D()
        {
            NewComponent<Terrain2D>();
        }

        [MenuItem("Tools/ScriptBoy/Digable2DTerrain/RoundCorner", false, 1)]
        public static void AddRoundCorner()
        {
            NewComponent<RoundCorner>();
        }

        [MenuItem("Tools/ScriptBoy/Digable2DTerrain/Shovel", false, 2)]
        public static void AddShovel()
        {
            NewComponent<Shovel>();
        }

        [MenuItem("Tools/ScriptBoy/Digable2DTerrain/Terrain2D", true, 0)]
        [MenuItem("Tools/ScriptBoy/Digable2DTerrain/Shovel", true, 2)]
        public static bool SelectionActiveGameObject()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("Tools/ScriptBoy/Digable2DTerrain/RoundCorner", true, 1)]
        public static bool IsGround2D()
        {
            GameObject g = Selection.activeGameObject;
            return g != null && g.GetComponent<Terrain2D>() != null;
        }

        public static void NewComponent<T>()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.AddComponent(typeof(T));
            }
        }
    }
}