using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace ScriptBoy.Digable2DTerrain
{
    public class Digable2DTerrainUpdateManager
    {
        private static bool IsStandaloneWindows
        {
            get
            {
                var buildTarget = EditorUserBuildSettings.activeBuildTarget;
                return buildTarget == BuildTarget.StandaloneWindows64 || buildTarget == BuildTarget.StandaloneWindows;
            }
        }

        private static bool IsCPPTriangulatorWorking
        {
            get
            {
                try
                {
                    Vector2[] polygon = new Vector2[]
                    {
                        new Vector2(0,0),
                        new Vector2(1,0),
                        new Vector2(0,-1)
                    };

                    Triangulator.Triangulate_CPP(polygon);
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        [InitializeOnLoadMethod]
        private static void RemoveOldFiles()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            string[] files = new string[]
            {
                "Assets/ScriptBoy/Digable2DTerrain/Scripts/Plugins/Triangulator.dll",
                "Assets/ScriptBoy/Digable2DTerrain/Scripts/Editor/DllNotFoundException.cs"
            };

            foreach (var file in files)
            {
                Delete(file);
            }

            AssetDatabase.Refresh();

            foreach (var file in files)
            {
                AskToDelete(file);
            }
        }

        [InitializeOnLoadMethod]
        private static void ChangeTriangulateMethod()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            if (!IsStandaloneWindows || !IsCPPTriangulatorWorking)
            {
                string AssetsFolder = Application.dataPath;
                string Terrain2DPath = AssetsFolder + "/ScriptBoy/Digable2DTerrain/Scripts/Terrain2D.cs";

                if (File.ReadAllLines(Terrain2DPath)[0] != "//Edited")
                {
                    string[] lines = new string[]
                    {
                        "//Edited",
                        "using UnityEngine;",
                        "namespace ScriptBoy.Digable2DTerrain",
                        "{",
                        "    public class Terrain2D : Dll.D2DT.Terrain2D",
                        "    {",
                        "        public override int[] TriangulatePolygon(Vector2[] polygon)",
                        "        {",
                        "           // return Triangulator.Triangulate_CPP(polygon);//Triangulate with Native plug-in",
                        "           return Triangulator.Triangulate_CSharp(polygon);//Triangulate with Managed plug-in",
                        "        }",
                        "    }",
                        "}"
                    };

                    File.WriteAllLines(Terrain2DPath, lines);

                    /*
                    string[] files = new string[]
                    {
                        "Assets/ScriptBoy/Digable2DTerrain/Scripts/Plugins/x86/Triangulator.dll",
                        "Assets/ScriptBoy/Digable2DTerrain/Scripts/Plugins/x64/Triangulator.dll",
                    };

                    foreach (var file in files)
                    {
                        Delete(file);
                    }

                    AssetDatabase.Refresh();

                    foreach (var file in files)
                    {
                        AskToDelete(file);
                    }
                    */
                }
            }
        }

        private static void Delete(string file)
        {
            if (File.Exists(file))
            {
                FileUtil.DeleteFileOrDirectory(file);
            }
        }

        private static void AskToDelete(string file)
        {
            if (File.Exists(file))//Exists yet !
            {
                EditorUtility.DisplayDialog("Digable2DTerrain", "Delete : " + file, "OK");
            }
        }
    }
}